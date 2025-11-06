using ConsultTechApp.Web.Services.Dtos.Application.Skill;

namespace ConsultTechApp.Web.Services.Dtos.Application.Category
{
    public class CategoryDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public IEnumerable<SkillDto> Skills { get; set; } = new List<SkillDto>();
    }
}
