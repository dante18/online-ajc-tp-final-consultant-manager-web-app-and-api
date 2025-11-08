using ConsultTechApp.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultTechApp.Api.Dtos.Skill
{
    public class SkillDetailDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;


    }
}
