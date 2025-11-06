using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Dtos.Application.Category;
using System.Net.Http.Json;

namespace ConsultTechApp.Web.Services.Services.Application
{
    public class CategoriesService : IApiCategoriesService
    {
        private readonly HttpClient httpClient;
        public CategoriesService(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory.CreateClient("Categories");
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await httpClient.GetFromJsonAsync<IEnumerable<CategoryDto>>("");
            return categories ?? [];
        }

        public async Task<CategoryDto?> GetCategoryAsync(Guid id)
        {
            var category = await httpClient.GetFromJsonAsync<CategoryDto>($"{id}");
            return category ?? new CategoryDto();
        }
    }
}
