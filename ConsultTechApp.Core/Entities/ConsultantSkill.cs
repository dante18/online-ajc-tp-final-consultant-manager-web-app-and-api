using ConsultTechApp.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultTechApp.Core.Entities;

[Table("ConsultantSkills")]
public class ConsultantSkill
{
    [Required]
    public Guid ConsultantId { get; set; }

    public Consultant Consultant { get; set; } = null!;

    [Required]
    public Guid SkillId { get; set; }

    public Skill Skill { get; set; } = null!;

    [Required]
    public ExpertiseLevel Level { get; set; }

    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
}

