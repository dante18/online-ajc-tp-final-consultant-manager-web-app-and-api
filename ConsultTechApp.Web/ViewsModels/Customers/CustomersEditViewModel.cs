using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ConsultTechApp.Web.ViewsModels.Customers;

public class CustomersEditViewModel
{
    [HiddenInput]
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
    [DataType(DataType.EmailAddress)]
    public string ContactEmail { get; set; }
}
