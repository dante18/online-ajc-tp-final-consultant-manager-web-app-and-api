using ConsultTechApp.Api.Dtos.Skill;

namespace ConsultTechApp.Api.Dtos.Category
{
    public class CategoryDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<SkillDto> Skills { get; set; } = new List<SkillDto>();
    }
}
