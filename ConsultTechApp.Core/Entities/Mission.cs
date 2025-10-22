using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultTechApp.Core.Entities;

[Table("Missions")]
public class Mission
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedBudget { get; set; }

    [ForeignKey(nameof(Customer))]
    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<MissionAssignment> Assignments { get; set; } = new List<MissionAssignment>();
}

