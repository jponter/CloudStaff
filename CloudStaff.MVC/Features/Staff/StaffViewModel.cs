using System.ComponentModel.DataAnnotations;

namespace CloudStaff.MVC.Features.Staff;

public class StaffViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Display(Name = "AS Number")]
    public string AsNumber { get; set; } = string.Empty;

    [Display(Name = "Manager")]
    public int ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;

    [Display(Name = "Home Pool")]
    public int? HomePoolId { get; set; }
    public string HomePoolName { get; set; } = string.Empty;

    [Display(Name = "Role")]
    public int? StaffRoleId { get; set; }
    public string StaffRoleName { get; set; } = string.Empty;

    [Display(Name = "GCM Level")]
    [Range(1, 9)]
    public int? GcmLevel { get; set; }

    [Display(Name = "SFIA Level")]
    [Range(1, 7)]
    public int? SfiaLevel { get; set; }

    [MaxLength(200)]
    [Display(Name = "Contract Job Title")]
    public string? ContractJobTitle { get; set; }

    public List<int> PlatformIds { get; set; } = new();
    public List<string> PlatformNames { get; set; } = new();
}
