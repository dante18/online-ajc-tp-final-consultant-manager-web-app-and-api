using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ConsultTechApp.Core.Entities;

[Table("Categories")]
public class Category
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}

