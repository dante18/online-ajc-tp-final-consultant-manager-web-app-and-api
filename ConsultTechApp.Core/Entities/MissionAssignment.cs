using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultTechApp.Core.Entities;

[Table("MissionAssignments")]
public class MissionAssignment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(Mission))]
    public Guid MissionId { get; set; }

    public Mission Mission { get; set; } = null!;

    [ForeignKey(nameof(Consultant))]
    public Guid ConsultantId { get; set; }

    public Consultant Consultant { get; set; } = null!;

    public DateTime AssignmentStart { get; set; }

    public DateTime? AssignmentEnd { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal DailyRate { get; set; }

    public string Role { get; set; } = string.Empty;
}

