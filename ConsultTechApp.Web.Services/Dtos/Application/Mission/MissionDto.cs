using ConsultTechApp.Web.Services.Dtos.Application.Consultant;
using ConsultTechApp.Web.Services.Dtos.Application.Customers;

namespace ConsultTechApp.Web.Services.Dtos.Application.Mission;

public class MissionDto
{
    public Guid? Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal EstimatedBudget { get; set; }

    public CustomerDto Customer { get; set; }

    public List<ConsultantDto>? Consultants { get; set; }
        = null;
}
