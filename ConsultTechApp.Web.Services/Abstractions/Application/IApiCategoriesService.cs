using ConsultTechApp.Web.Services.Dtos.Application.Category;

namespace ConsultTechApp.Web.Services.Abstractions.Application
{
    public interface IApiCategoriesService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> GetCategoryAsync(Guid id);
    }
}
