namespace CloudStaff.EntityModels;

public class StaffCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<HomePool> HomePools { get; set; } = [];
}
