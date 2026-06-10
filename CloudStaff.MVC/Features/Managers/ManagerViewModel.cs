using System.ComponentModel.DataAnnotations;

namespace CloudStaff.MVC.Features.Managers;

public class ManagerViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Display(Name = "AS Number")]
    public string AsNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public int StaffCount { get; set; }
}
