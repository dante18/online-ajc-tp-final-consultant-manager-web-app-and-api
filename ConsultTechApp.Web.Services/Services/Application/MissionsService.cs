using System.Net.Http.Json;
using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Dtos.Application.Mission;

namespace ConsultTechApp.Web.Services.Services.Application;

internal class MissionsService : IApiMissionsService
{
    private readonly HttpClient httpClient;

    public MissionsService(IHttpClientFactory httpClientFactory)
    {
        this.httpClient = httpClientFactory.CreateClient("Missions");
    }

    public async Task<IEnumerable<MissionDto>> GetMissionsAsync()
    {
        var missions = await httpClient.GetFromJsonAsync<IEnumerable<MissionDto>>("");
        return missions ?? [];
    }

    public async Task<MissionDto?> GetMissionAsync(Guid id)
    {
        var mission = await httpClient.GetFromJsonAsync<MissionDto>($"{id}");
        return mission;
    }

    public async Task CreateMissionAsync(CreateOrUpdateMissionDto missionDTO)
    {
        var response = await httpClient.PostAsJsonAsync("", missionDTO);
        _ = response.EnsureSuccessStatusCode();
    }

    public async Task UpdateMissionAsync(Guid id, CreateOrUpdateMissionDto missionDto)
    {
        var response = await httpClient.PutAsJsonAsync($"{id}", missionDto);
        _ = response.EnsureSuccessStatusCode();
    }

    public async Task DeleteMissionAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync($"{id}");
        _ = response.EnsureSuccessStatusCode();
    }
}
