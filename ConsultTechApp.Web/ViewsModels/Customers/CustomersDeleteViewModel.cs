namespace ConsultTechApp.Web.ViewsModels.Customers;

public class CustomersDeleteViewModel
{
    public Guid? Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public string Industry { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;
}
