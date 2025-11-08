using ConsultTechApp.Core.Entities;

namespace ConsultTechApp.Core.Abstractions.Repositories
{
    public interface ISkillRepository
    {
        public List<Skill> GetAllSkills();

        public Skill GetSkill(Guid id);

        public void CreateSkill(Skill skill);

        public void UpdateSkill(Skill skill);

        public void DeleteSkill(Skill skill);
    }
}
