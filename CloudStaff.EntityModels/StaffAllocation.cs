namespace CloudStaff.EntityModels;

/// <summary>
/// A single allocation of a staff member to a project over a date range.
/// StartDate and EndDate are the source of truth — both weekly and monthly
/// timeline views derive their cell values by checking date overlap.
/// </summary>
public class StaffAllocation
{
    public int Id { get; set; }

    public int StaffId { get; set; }
    public Staff Staff { get; set; } = null!;

    public int ClientProjectId { get; set; }
    public ClientProject ClientProject { get; set; } = null!;

    /// <summary>Inclusive start date of this allocation.</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>Inclusive end date of this allocation.</summary>
    public DateOnly EndDate { get; set; }

    /// <summary>Percentage allocated (0–100+). Values &gt;100 trigger over-allocation warning.</summary>
    public int Percentage { get; set; }
}
