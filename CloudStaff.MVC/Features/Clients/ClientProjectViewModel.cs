using CloudStaff.EntityModels;
using System.ComponentModel.DataAnnotations;

namespace CloudStaff.MVC.Features.Clients;

public class ClientProjectViewModel
{
    public int Id { get; set; }
    public int ClientId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public string? ProjectManagerName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Display(Name = "Status")]
    public ClientProjectStatus Status { get; set; } = ClientProjectStatus.Active;
}
