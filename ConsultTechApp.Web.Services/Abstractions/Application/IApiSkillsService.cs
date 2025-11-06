using ConsultTechApp.Web.Services.Dtos.Application.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultTechApp.Web.Services.Abstractions.Application
{
    public interface IApiSkillsService
    {
        public Task<IEnumerable<SkillDto>> GetSkillsAsync();

        public Task<SkillDetailDto?> GetSkillAsync(Guid id);

        public Task CreateSkillAsync(CreateOrUpdateSkillDto skillDto);

        public Task UpdateSkillAsync(Guid id, CreateOrUpdateSkillDto skillDto);

        public Task DeleteSkillAsync(Guid id);
    }
}
