namespace ConsultTechApp.Web.ViewsModels.Skills
{
    public class SkillsViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
