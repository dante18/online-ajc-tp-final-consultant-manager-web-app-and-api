namespace ConsultTechApp.Api.Dtos.Customer;

public class CustomerDto
{
    public Guid? Id { get; set; }

    public string CompanyName { get; set; }

    public string Industry { get; set; }

    public string Address { get; set; }

    public string ContactName { get; set; }

    public string ContactEmail { get; set; }
}
