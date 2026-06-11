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
}
