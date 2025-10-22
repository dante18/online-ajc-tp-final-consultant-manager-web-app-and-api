using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultTechApp.Core.Entities;

[Table("Customers")]
public class Customer
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = null!;

    public string Industry { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public List<Mission> Missions { get; set; } = new List<Mission>();
}
