namespace CloudStaff.EntityModels;

public class HomePool
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public StaffCategory Category { get; set; } = null!;

    public ICollection<StaffRole> Roles { get; set; } = [];
    public ICollection<Staff> Staff { get; set; } = [];
}
