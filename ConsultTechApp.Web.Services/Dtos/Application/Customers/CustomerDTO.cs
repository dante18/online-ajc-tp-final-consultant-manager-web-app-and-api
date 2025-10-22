using System.ComponentModel;

namespace ConsultTechApp.Web.Services.Dtos.Application.Customers;

public class CustomerDto
{
    public Guid? Id { get; set; }

    [DisplayName("Customer Name")]
    public string CompanyName { get; set; }

    [DisplayName("Sector of activity")]
    public string Industry { get; set; }

    [DisplayName("Address")]
    public string Address { get; set; }

    [DisplayName("Contact Name")]
    public string ContactName { get; set; }

    [DisplayName("Contact Email")]
    public string ContactEmail { get; set; }
}
