namespace CloudStaff.EntityModels;

public class Staff
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AsNumber { get; set; } = string.Empty;

    public int ManagerId { get; set; }
    public Manager Manager { get; set; } = null!;

    public int? HomePoolId { get; set; }
    public HomePool? HomePool { get; set; }

    public int? StaffRoleId { get; set; }
    public StaffRole? StaffRole { get; set; }

    public int? GcmLevel { get; set; }
    public int? SfiaLevel { get; set; }

    public string? ContractJobTitle { get; set; }

    public ICollection<StaffPlatform> StaffPlatforms { get; set; } = [];
}
