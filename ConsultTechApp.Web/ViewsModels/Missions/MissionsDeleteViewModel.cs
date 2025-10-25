namespace ConsultTechApp.Web.ViewsModels.Missions;

public class MissionsDeleteViewModel
{
    public Guid? Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal EstimatedBudget { get; set; }

    public string Customer { get; set; } = string.Empty;
}
