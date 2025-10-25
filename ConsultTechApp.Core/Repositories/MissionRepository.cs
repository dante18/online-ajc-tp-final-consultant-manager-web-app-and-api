using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Context;
using ConsultTechApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsultTechApp.Core.Repositories;

internal class MissionRepository : IMissionRepository
{
    private readonly ApplicationStoreContext _context;

    public MissionRepository(ApplicationStoreContext context)
    {
        _context = context;
    }

    public List<Mission> GetAllMissions()
    {
        return _context.Missions
            .Include(m => m.Customer)
            .Include(m => m.Assignments)
            .ThenInclude(a => a.Consultant)
            .ToList();
    }

    public Mission GetMission(Guid id)
    {
        return _context.Missions
            .Include(m => m.Customer)
            .Include(m => m.Assignments)
            .ThenInclude(a => a.Consultant)
            .FirstOrDefault(m => m.Id == id);
    }

    public void CreateMission(Mission mission)
    {
        _context.Missions.Add(mission);
        _context.SaveChanges();
    }

    public void UpdateMission(Mission mission)
    {
        _context.Missions.Update(mission);
        _context.SaveChanges();
    }

    public void DeleteMission(Mission mission)
    {
        _context.Missions.Remove(mission);
        _context.SaveChanges();
    }
}