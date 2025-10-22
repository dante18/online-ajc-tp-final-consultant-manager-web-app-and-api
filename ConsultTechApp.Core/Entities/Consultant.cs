using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultTechApp.Core.Entities;

[Table("Consultants")]
public class Consultant
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required]
    public DateTime HireDate { get; set; }

    [DefaultValue(true)]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ConsultantSkill> ConsultantSkills { get; set; } = new List<ConsultantSkill>();

    public ICollection<MissionAssignment> MissionAssignments { get; set; } = new List<MissionAssignment>();
}

