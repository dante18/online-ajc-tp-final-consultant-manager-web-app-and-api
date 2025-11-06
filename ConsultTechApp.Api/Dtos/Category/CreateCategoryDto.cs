namespace ConsultTechApp.Api.Dtos.Category
{
    public class CreateCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
