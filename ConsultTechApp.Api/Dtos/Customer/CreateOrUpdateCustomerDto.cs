using System.ComponentModel.DataAnnotations;

namespace ConsultTechApp.Api.Dtos.Customer;

public class CreateOrUpdateCustomerDto
{
    [Required]
    [StringLength(200)]
    public string CompanyName { get; set; }

    [Required]
    public string Industry { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string ContactName { get; set; }

    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; }
}
