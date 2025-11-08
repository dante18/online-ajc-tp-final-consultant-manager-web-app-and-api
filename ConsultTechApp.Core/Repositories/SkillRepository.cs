using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Context;
using ConsultTechApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsultTechApp.Core.Repositories
{
    internal class SkillRepository : ISkillRepository
    {
        private readonly ApplicationStoreContext _context;
        public SkillRepository(ApplicationStoreContext context) 
        {
            _context = context;
        }

        public List<Skill> GetAllSkills()
        {
            return _context.Skills
                .Include(s => s.Category)
                .Include(s => s.ConsultantSkills)
                .ThenInclude(cs => cs.Consultant)
                .ToList();
        }

        public Skill GetSkill(Guid id)
        {
            return _context.Skills
                .Include(s => s.Category)
                .Include(s => s.ConsultantSkills)
                .ThenInclude(cs => cs.Consultant)
                .FirstOrDefault(s => s.Id == id);
        }
        public void CreateSkill(Skill skill)
        {
            _context.Skills.Add(skill);
            _context.SaveChanges();
        }

        public void UpdateSkill(Skill skill)
        {
            _context.Skills.Update(skill);
            _context.SaveChanges();
        }

        public void DeleteSkill(Skill skill)
        {
            _context.Skills.Remove(skill);
            _context.SaveChanges();
        }
    }
}
