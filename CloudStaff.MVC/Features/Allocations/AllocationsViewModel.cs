namespace CloudStaff.MVC.Features.Allocations;

public class AllocationsViewModel
{
    public IList<ManagerOption> Managers { get; set; } = new List<ManagerOption>();
    public int SelectedManagerId { get; set; }
    public string Mode { get; set; } = "weeks";
    public int Year { get; set; }
    public int StartPeriod { get; set; }

    // Navigation targets
    public int PrevYear { get; set; }
    public int PrevStartPeriod { get; set; }
    public int NextYear { get; set; }
    public int NextStartPeriod { get; set; }

    public IList<PeriodHeader> Periods { get; set; } = new List<PeriodHeader>();
    public IList<StaffAllocationRow> StaffRows { get; set; } = new List<StaffAllocationRow>();
}

public record ManagerOption(int Id, string Name);

public class PeriodHeader
{
    public int Year { get; set; }
    public int PeriodNumber { get; set; }
    public string Label { get; set; } = string.Empty;      // "Wk 24"  or  "Jun"
    public string SubLabel { get; set; } = string.Empty;   // "27 May" or  "2026"
    public DateOnly StartDate { get; set; }                // First day of period
    public DateOnly EndDate { get; set; }                  // Last day of period
}

public class StaffAllocationRow
{
    public int StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string AsNumber { get; set; } = string.Empty;
    public IList<PeriodAllocation> Cells { get; set; } = new List<PeriodAllocation>();
}

public class PeriodAllocation
{
    public int Year { get; set; }
    public int PeriodNumber { get; set; }
    public int TotalPercent { get; set; }

    public string ColorClass => TotalPercent switch
    {
        0     => "alloc-none",
        <= 25 => "alloc-low",
        <= 50 => "alloc-mid",
        <= 75 => "alloc-good",
        <= 99 => "alloc-high",
        100   => "alloc-full",
        _     => "alloc-over"
    };
}
