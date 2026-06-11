using System.Globalization;
using CloudStaff.DataContext;
using CloudStaff.EntityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudStaff.MVC.Features.Allocations;

public class AllocationsController : Controller
{
    private readonly CloudStaffContext _db;

    public AllocationsController(CloudStaffContext db) => _db = db;

    // ── Index ─────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Index(int? managerId, string mode = "weeks",
                                           int? year = null, int? startPeriod = null)
    {
        mode = mode == "months" ? "months" : "weeks";
        bool isWeeks = mode == "weeks";
        var today = DateTime.Today;

        int effectiveYear  = year        ?? (isWeeks ? ISOWeek.GetYear(today) : today.Year);
        int effectiveStart = startPeriod ?? (isWeeks ? ISOWeek.GetWeekOfYear(today) : 1);
        if (!isWeeks) effectiveStart = 1;

        var periods = isWeeks
            ? ComputeWeekPeriods(effectiveYear, effectiveStart, 13)
            : ComputeMonthPeriods(effectiveYear);

        int prevYear, prevStart, nextYear, nextStart;
        if (isWeeks)
        {
            (prevYear, prevStart) = ShiftWeeks(effectiveYear, effectiveStart, -13);
            (nextYear, nextStart) = ShiftWeeks(effectiveYear, effectiveStart, +13);
        }
        else
        {
            (prevYear, prevStart) = (effectiveYear - 1, 1);
            (nextYear, nextStart) = (effectiveYear + 1, 1);
        }

        var managers = await _db.Managers.OrderBy(m => m.Name).ToListAsync();
        int selectedId = managerId ?? managers.FirstOrDefault()?.Id ?? 0;

        var staff = await _db.Staff
            .Where(s => s.ManagerId == selectedId)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var staffIds   = staff.Select(s => s.Id).ToList();
        var rangeStart = periods.First().StartDate;
        var rangeEnd   = periods.Last().EndDate;

        // Load only allocations whose date range overlaps with the visible window
        var allocations = staffIds.Any()
            ? await _db.StaffAllocations
                .Include(a => a.ClientProject).ThenInclude(p => p.Client)
                .Where(a => staffIds.Contains(a.StaffId)
                            && a.StartDate <= rangeEnd
                            && a.EndDate >= rangeStart)
                .ToListAsync()
            : new List<StaffAllocation>();

        var vm = new AllocationsViewModel
        {
            Managers          = managers.Select(m => new ManagerOption(m.Id, m.Name)).ToList(),
            SelectedManagerId = selectedId,
            Mode              = mode,
            Year              = effectiveYear,
            StartPeriod       = effectiveStart,
            PrevYear          = prevYear,
            PrevStartPeriod   = prevStart,
            NextYear          = nextYear,
            NextStartPeriod   = nextStart,
            Periods           = periods,
            StaffRows         = staff.Select(s => new StaffAllocationRow
            {
                StaffId   = s.Id,
                StaffName = s.Name,
                AsNumber  = s.AsNumber,
                Cells     = periods.Select(p => BuildCell(s.Id, p, allocations)).ToList()
            }).ToList()
        };

        return View(vm);
    }

    // ── CellData ──────────────────────────────────────────────────────────────
    public async Task<IActionResult> CellData(int staffId, DateOnly periodStart, DateOnly periodEnd)
    {
        var allocs = await _db.StaffAllocations
            .Include(a => a.ClientProject).ThenInclude(p => p.Client)
            .Where(a => a.StaffId == staffId
                        && a.StartDate <= periodEnd
                        && a.EndDate >= periodStart)
            .OrderBy(a => a.StartDate)
            .ToListAsync();

        int total = allocs.Sum(a => a.Percentage);

        var projects = await _db.ClientProjects
            .Include(p => p.Client)
            .OrderBy(p => p.Client.Name).ThenBy(p => p.Name)
            .Select(p => new { id = p.Id, name = p.Name, clientName = p.Client.Name })
            .ToListAsync();

        return Json(new
        {
            allocations = allocs.Select(a => new
            {
                id          = a.Id,
                projectId   = a.ClientProjectId,
                projectName = a.ClientProject.Name,
                clientName  = a.ClientProject.Client?.Name ?? string.Empty,
                percentage  = a.Percentage,
                startDate   = a.StartDate.ToString("yyyy-MM-dd"),
                endDate     = a.EndDate.ToString("yyyy-MM-dd")
            }),
            projects,
            total
        });
    }

    // ── SetAllocation ─────────────────────────────────────────────────────────
    [HttpPost, IgnoreAntiforgeryToken]
    public async Task<IActionResult> SetAllocation([FromBody] SetAllocationRequest req)
    {
        if (req.EndDate < req.StartDate)
            return BadRequest("EndDate must be on or after StartDate.");

        // Upsert: same staff + project + exact date range → update percentage
        var existing = await _db.StaffAllocations.FirstOrDefaultAsync(a =>
            a.StaffId         == req.StaffId         &&
            a.ClientProjectId == req.ClientProjectId &&
            a.StartDate       == req.StartDate       &&
            a.EndDate         == req.EndDate);

        if (existing is not null)
            existing.Percentage = req.Percentage;
        else
            _db.StaffAllocations.Add(new StaffAllocation
            {
                StaffId         = req.StaffId,
                ClientProjectId = req.ClientProjectId,
                StartDate       = req.StartDate,
                EndDate         = req.EndDate,
                Percentage      = req.Percentage
            });

        await _db.SaveChangesAsync();

        // Return updated total for the clicked cell so the caller knows to reload
        int total = await _db.StaffAllocations
            .Where(a => a.StaffId == req.StaffId
                        && a.StartDate <= req.PeriodEnd
                        && a.EndDate >= req.PeriodStart)
            .SumAsync(a => a.Percentage);

        return Json(new { success = true, total });
    }

    // ── DeleteAllocation ──────────────────────────────────────────────────────
    [HttpPost, IgnoreAntiforgeryToken]
    public async Task<IActionResult> DeleteAllocation([FromBody] DeleteAllocationRequest req)
    {
        var a = await _db.StaffAllocations.FindAsync(req.Id);
        if (a is null) return NotFound();

        int staffId = a.StaffId;
        _db.StaffAllocations.Remove(a);
        await _db.SaveChangesAsync();

        int total = await _db.StaffAllocations
            .Where(x => x.StaffId == staffId
                        && x.StartDate <= req.PeriodEnd
                        && x.EndDate >= req.PeriodStart)
            .SumAsync(x => x.Percentage);

        return Json(new { success = true, total });
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static List<PeriodHeader> ComputeWeekPeriods(int year, int startWeek, int count)
    {
        var list = new List<PeriodHeader>(count);
        int w = startWeek, y = year;
        for (int i = 0; i < count; i++)
        {
            var monday = ISOWeek.ToDateTime(y, w, DayOfWeek.Monday);
            list.Add(new PeriodHeader
            {
                Year         = y,
                PeriodNumber = w,
                Label        = $"Wk {w}",
                SubLabel     = monday.ToString("d MMM"),
                StartDate    = DateOnly.FromDateTime(monday),
                EndDate      = DateOnly.FromDateTime(monday.AddDays(6))
            });
            w++;
            if (w > ISOWeek.GetWeeksInYear(y)) { w = 1; y++; }
        }
        return list;
    }

    private static List<PeriodHeader> ComputeMonthPeriods(int year) =>
        Enumerable.Range(1, 12).Select(m => new PeriodHeader
        {
            Year         = year,
            PeriodNumber = m,
            Label        = new DateTime(year, m, 1).ToString("MMM"),
            SubLabel     = year.ToString(),
            StartDate    = new DateOnly(year, m, 1),
            EndDate      = new DateOnly(year, m, DateTime.DaysInMonth(year, m))
        }).ToList();

    private static (int year, int week) ShiftWeeks(int year, int week, int delta)
    {
        var date = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday).AddDays(delta * 7);
        return (ISOWeek.GetYear(date), ISOWeek.GetWeekOfYear(date));
    }

    /// <summary>
    /// Cell total = sum of all allocations whose date range overlaps with the period.
    /// Same logic applies to both weekly and monthly cells.
    /// </summary>
    private static PeriodAllocation BuildCell(int staffId, PeriodHeader period,
                                              List<StaffAllocation> all)
    {
        int total = all
            .Where(a => a.StaffId == staffId
                        && a.StartDate <= period.EndDate
                        && a.EndDate >= period.StartDate)
            .Sum(a => a.Percentage);

        return new PeriodAllocation
        {
            Year         = period.Year,
            PeriodNumber = period.PeriodNumber,
            TotalPercent = total
        };
    }
}

// ── Request / response models ─────────────────────────────────────────────────

public record SetAllocationRequest(
    int      StaffId,
    int      ClientProjectId,
    DateOnly StartDate,
    DateOnly EndDate,
    int      Percentage,
    DateOnly PeriodStart,   // boundaries of the clicked cell (for return total)
    DateOnly PeriodEnd);

public record DeleteAllocationRequest(
    int      Id,
    DateOnly PeriodStart,   // boundaries of the clicked cell (for return total)
    DateOnly PeriodEnd);
