using System.ComponentModel.DataAnnotations;

namespace ConsultTechApp.Api.Dtos.Skill
{
    public class SkillDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
