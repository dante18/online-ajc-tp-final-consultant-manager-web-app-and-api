using ConsultTechApp.Web.ViewsModels.Consultants;
using ConsultTechApp.Web.ViewsModels.Customers;

namespace ConsultTechApp.Web.ViewsModels.Missions;

public class MissionsDetailsViewModel
{
    public Guid? Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal EstimatedBudget { get; set; }

    public CustomersViewModel Customer { get; set; }

    public List<ConsultantsViewModel> Consultants { get; set; }
}
