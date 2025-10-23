using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConsultTechApp.Web.Services.Dtos.Application.Customers;

public class CreateOrUpdateCustomerDto
{
    public Guid? Id { get; set; }

    [DisplayName("Customer Name")]
    [StringLength(200)]
    public string CompanyName { get; set; }

    [DisplayName("Sector of activity")]
    public string Industry { get; set; }

    [DisplayName("Address")]
    public string Address { get; set; }

    [DisplayName("Contact Name")]
    public string ContactName { get; set; }

    [DisplayName("Contact Email")]
    [EmailAddress]
    public string ContactEmail { get; set; }

}
