namespace CloudStaff.EntityModels;

public class StaffPlatform
{
    public int StaffId { get; set; }
    public Staff Staff { get; set; } = null!;

    public int PlatformId { get; set; }
    public Platform Platform { get; set; } = null!;
}
