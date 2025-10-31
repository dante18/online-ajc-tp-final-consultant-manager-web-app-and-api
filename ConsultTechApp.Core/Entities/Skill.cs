using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ConsultTechApp.Core.Entities;

[Table("Skills")]
public class Skill
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    [ForeignKey(nameof(Category))]
    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    [JsonIgnore]
    public ICollection<ConsultantSkill> ConsultantSkills { get; set; } = new List<ConsultantSkill>();
}
