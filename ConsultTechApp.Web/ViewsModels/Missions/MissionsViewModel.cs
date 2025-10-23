namespace ConsultTechApp.Web.ViewsModels.Missions;

public class MissionsViewModel
{
    public Guid? Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal EstimatedBudget { get; set; }
}
