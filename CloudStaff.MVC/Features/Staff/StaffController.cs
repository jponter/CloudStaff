using CloudStaff.DataContext;
using CloudStaff.EntityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CloudStaff.MVC.Features.Staff;

public class StaffController : Controller
{
    private readonly CloudStaffContext _db;

    public StaffController(CloudStaffContext db)
    {
        _db = db;
    }

    private async Task PopulateDropdowns(int? selectedHomePoolId = null)
    {
        var managers = await _db.Managers.OrderBy(m => m.Name).ToListAsync();
        ViewBag.Managers = new SelectList(managers, "Id", "Name");

        var homePools = await _db.HomePools
            .Include(h => h.Category)
            .OrderBy(h => h.Category.Name)
            .ThenBy(h => h.Name)
            .ToListAsync();

        var groups = homePools
            .Select(h => h.Category.Name)
            .Distinct()
            .ToDictionary(name => name, name => new SelectListGroup { Name = name });

        ViewBag.HomePools = homePools.Select(h => new SelectListItem
        {
            Value = h.Id.ToString(),
            Text = h.Name,
            Group = groups[h.Category.Name]
        }).ToList();

        var roles = selectedHomePoolId.HasValue
            ? await _db.StaffRoles
                .Where(r => r.HomePoolId == selectedHomePoolId)
                .OrderBy(r => r.Title)
                .ToListAsync()
            : new List<StaffRole>();

        ViewBag.StaffRoles = new SelectList(roles, "Id", "Title");
        ViewBag.Platforms = await _db.Platforms.OrderBy(p => p.Name).ToListAsync();
    }

    // GET /Staff
    public async Task<IActionResult> Index()
    {
        var staff = await _db.Staff
            .Select(s => new StaffViewModel
            {
                Id = s.Id,
                Name = s.Name,
                AsNumber = s.AsNumber,
                ManagerId = s.ManagerId,
                ManagerName = s.Manager.Name,
                HomePoolId = s.HomePoolId,
                HomePoolName = s.HomePool != null ? s.HomePool.Name : string.Empty,
                StaffRoleId = s.StaffRoleId,
                StaffRoleName = s.StaffRole != null ? s.StaffRole.Title : string.Empty,
                GcmLevel = s.GcmLevel,
                SfiaLevel = s.SfiaLevel,
            })
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(staff);
    }

    // GET /Staff/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View(new StaffViewModel());
    }

    // POST /Staff/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StaffViewModel vm)
    {
        if (await _db.Staff.AnyAsync(s => s.AsNumber == vm.AsNumber))
            ModelState.AddModelError(nameof(vm.AsNumber), "AS Number is already in use.");

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(vm.HomePoolId);
            return View(vm);
        }

        var staff = new CloudStaff.EntityModels.Staff
        {
            Name = vm.Name,
            AsNumber = vm.AsNumber,
            ManagerId = vm.ManagerId,
            HomePoolId = vm.HomePoolId,
            StaffRoleId = vm.StaffRoleId,
            GcmLevel = vm.GcmLevel,
            SfiaLevel = vm.SfiaLevel,
            ContractJobTitle = vm.ContractJobTitle,
        };
        _db.Staff.Add(staff);
        await _db.SaveChangesAsync();

        foreach (var platformId in vm.PlatformIds)
            _db.StaffPlatforms.Add(new StaffPlatform { StaffId = staff.Id, PlatformId = platformId });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET /Staff/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var staff = await _db.Staff
            .Include(s => s.StaffPlatforms)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (staff is null) return NotFound();

        var vm = new StaffViewModel
        {
            Id = staff.Id,
            Name = staff.Name,
            AsNumber = staff.AsNumber,
            ManagerId = staff.ManagerId,
            HomePoolId = staff.HomePoolId,
            StaffRoleId = staff.StaffRoleId,
            GcmLevel = staff.GcmLevel,
            SfiaLevel = staff.SfiaLevel,
            ContractJobTitle = staff.ContractJobTitle,
            PlatformIds = staff.StaffPlatforms.Select(sp => sp.PlatformId).ToList(),
        };

        await PopulateDropdowns(staff.HomePoolId);
        return View(vm);
    }

    // POST /Staff/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StaffViewModel vm)
    {
        if (await _db.Staff.AnyAsync(s => s.AsNumber == vm.AsNumber && s.Id != id))
            ModelState.AddModelError(nameof(vm.AsNumber), "AS Number is already in use.");

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(vm.HomePoolId);
            return View(vm);
        }

        var staff = await _db.Staff
            .Include(s => s.StaffPlatforms)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (staff is null) return NotFound();

        staff.Name = vm.Name;
        staff.AsNumber = vm.AsNumber;
        staff.ManagerId = vm.ManagerId;
        staff.HomePoolId = vm.HomePoolId;
        staff.StaffRoleId = vm.StaffRoleId;
        staff.GcmLevel = vm.GcmLevel;
        staff.SfiaLevel = vm.SfiaLevel;
        staff.ContractJobTitle = vm.ContractJobTitle;

        _db.StaffPlatforms.RemoveRange(staff.StaffPlatforms);
        foreach (var platformId in vm.PlatformIds)
            _db.StaffPlatforms.Add(new StaffPlatform { StaffId = id, PlatformId = platformId });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET /Staff/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var staff = await _db.Staff
            .Include(s => s.Manager)
            .Include(s => s.HomePool)
            .Include(s => s.StaffRole)
            .Include(s => s.StaffPlatforms)
                .ThenInclude(sp => sp.Platform)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (staff is null) return NotFound();

        var vm = new StaffViewModel
        {
            Id = staff.Id,
            Name = staff.Name,
            AsNumber = staff.AsNumber,
            ManagerId = staff.ManagerId,
            ManagerName = staff.Manager.Name,
            HomePoolId = staff.HomePoolId,
            HomePoolName = staff.HomePool?.Name ?? string.Empty,
            StaffRoleId = staff.StaffRoleId,
            StaffRoleName = staff.StaffRole?.Title ?? string.Empty,
            GcmLevel = staff.GcmLevel,
            SfiaLevel = staff.SfiaLevel,
            ContractJobTitle = staff.ContractJobTitle,
            PlatformNames = staff.StaffPlatforms.Select(sp => sp.Platform.Name).ToList(),
        };

        return View(vm);
    }

    // POST /Staff/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var staff = await _db.Staff.FindAsync(id);
        if (staff is null) return NotFound();

        _db.Staff.Remove(staff);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET /Staff/GetRolesByPool?homePoolId=X
    public async Task<IActionResult> GetRolesByPool(int homePoolId)
    {
        var roles = await _db.StaffRoles
            .Where(r => r.HomePoolId == homePoolId)
            .OrderBy(r => r.Title)
            .Select(r => new { id = r.Id, title = r.Title })
            .ToListAsync();

        return Json(roles);
    }
}
