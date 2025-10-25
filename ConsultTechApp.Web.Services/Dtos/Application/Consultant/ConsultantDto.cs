namespace ConsultTechApp.Web.Services.Dtos.Application.Consultant;

public class ConsultantDto
{
    public Guid? Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; }
}
