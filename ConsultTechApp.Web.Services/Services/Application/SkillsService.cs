using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Dtos.Application.Skill;
using System.Net.Http.Json;

namespace ConsultTechApp.Web.Services.Services.Application
{
    public class SkillsService : IApiSkillsService
    {
        private readonly HttpClient httpClient;
        public SkillsService(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory.CreateClient("Skills");
        }

        public async Task<IEnumerable<SkillDto>> GetSkillsAsync()
        {
            var skills = await httpClient.GetFromJsonAsync<IEnumerable<SkillDto>>("");
            return skills ?? [];
        }

        public async Task<SkillDetailDto?> GetSkillAsync(Guid id)
        {
            var skill = await httpClient.GetFromJsonAsync<SkillDetailDto>($"{id}");
            return skill;
        }

        public async Task CreateSkillAsync(CreateOrUpdateSkillDto skillDto)
        {
            var response = await httpClient.PostAsJsonAsync("", skillDto);
            _ = response.EnsureSuccessStatusCode();
        }

        public async Task UpdateSkillAsync(Guid id, CreateOrUpdateSkillDto skillDto)
        {
            var response = await httpClient.PutAsJsonAsync($"{id}", skillDto);
            _ = response.EnsureSuccessStatusCode();
        }

        public async Task DeleteSkillAsync(Guid id)
        {
            var response = await httpClient.DeleteAsync($"{id}");
            _ = response.EnsureSuccessStatusCode();
        }
    }
}
