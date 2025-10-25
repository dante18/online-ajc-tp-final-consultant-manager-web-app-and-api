using ConsultTechApp.Core.Entities;

namespace ConsultTechApp.Core.Abstractions.Repositories;

public interface IMissionRepository
{
    public List<Mission> GetAllMissions();

    public Mission GetMission(Guid id);

    public void CreateMission(Mission mission);

    public void UpdateMission(Mission mission);

    public void DeleteMission(Mission mission);
}