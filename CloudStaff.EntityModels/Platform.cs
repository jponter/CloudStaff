namespace CloudStaff.EntityModels;

public class Platform
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<StaffPlatform> StaffPlatforms { get; set; } = [];
}
