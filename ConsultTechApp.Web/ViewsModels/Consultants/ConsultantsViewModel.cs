namespace ConsultTechApp.Web.ViewsModels.Consultants;

public class ConsultantsViewModel
{
    public Guid? Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; }
}
