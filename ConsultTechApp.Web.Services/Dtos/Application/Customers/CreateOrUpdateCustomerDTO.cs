using System.ComponentModel.DataAnnotations;

namespace ConsultTechApp.Web.Services.Dtos.Application.Customers;

public class CreateOrUpdateCustomerDto
{

    [StringLength(200)]
    [Required]
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
