using CloudStaff.EntityModels;
using System.ComponentModel.DataAnnotations;

namespace CloudStaff.MVC.Features.Clients;

public class ClientViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Display(Name = "Type")]
    public ClientType Type { get; set; } = ClientType.External;

    public int ProjectCount { get; set; }

    public string? PrimaryContactName { get; set; }
    public string? PrimaryContactEmail { get; set; }
    public string? AccountManagerName { get; set; }
    public string? AccountManagerEmail { get; set; }
    public string? ExecutiveSponsorName { get; set; }
    public string? ExecutiveSponsorEmail { get; set; }

    [Display(Name = "Status")]
    public ClientStatus Status { get; set; } = ClientStatus.Active;
}
