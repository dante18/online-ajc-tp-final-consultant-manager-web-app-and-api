using ConsultTechApp.Web.Services.Dtos.Application.Mission;

namespace ConsultTechApp.Web.Services.Abstractions.Application;

public interface IApiMissionsService
{
    public Task<IEnumerable<MissionDto>> GetMissionsAsync();

    public Task<MissionDto?> GetMissionAsync(Guid id);

    public Task CreateMissionAsync(CreateOrUpdateMissionDto missionDto);

    public Task UpdateMissionAsync(Guid id, CreateOrUpdateMissionDto missionDto);

    public Task DeleteMissionAsync(Guid id);
}