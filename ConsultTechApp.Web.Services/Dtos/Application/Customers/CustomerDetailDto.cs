using ConsultTechApp.Web.Services.Dtos.Application.Mission;

namespace ConsultTechApp.Web.Services.Dtos.Application.Customers;

public class CustomerDetailDto
{
    public Guid? Id { get; set; }

    public string CompanyName { get; set; }

    public string Industry { get; set; }

    public string Address { get; set; }

    public string ContactName { get; set; }

    public string ContactEmail { get; set; }

    public List<MissionDto> Missions { get; set; } = new List<MissionDto>();
}
