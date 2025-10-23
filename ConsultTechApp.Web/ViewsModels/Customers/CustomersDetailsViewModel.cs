using ConsultTechApp.Web.ViewsModels.Missions;

namespace ConsultTechApp.Web.ViewsModels.Customers;

public class CustomersDetailsViewModel
{
    public Guid? Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public string Industry { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public List<MissionsViewModel> Missions { get; set; }
}
