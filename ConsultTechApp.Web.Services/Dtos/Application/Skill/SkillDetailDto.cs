namespace ConsultTechApp.Web.Services.Dtos.Application.Skill
{
    public class SkillDetailDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;


    }
}
