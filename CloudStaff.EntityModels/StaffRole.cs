namespace CloudStaff.EntityModels;

public class StaffRole
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public int HomePoolId { get; set; }
    public HomePool HomePool { get; set; } = null!;

    public ICollection<Staff> Staff { get; set; } = [];
}
