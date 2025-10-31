using ConsultTechApp.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace ConsultTechApp.Api.Dtos.Skill
{
    public class CreateOrUpdateSkillDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        public Guid CategoryId { get; set; } 
    }
}
