using ConsultTechApp.Api.Dtos.Skill;
using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConsultTechApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : Controller
    {
        private readonly ILogger<SkillsController> logger;
        private readonly ISkillRepository skillRepository;
        public SkillsController(ILogger<SkillsController> logger, ISkillRepository skillRepository)
        {
            this.logger = logger;
            this.skillRepository = skillRepository;
            this.logger.LogInformation("SkillsController initialized");
        }

        [HttpGet]
        public IResult GetSkills()
        {
            this.logger.LogInformation("GetSkills called");
            try
            {
                var skills = this.skillRepository.GetAllSkills().Select(s => new SkillDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    CategoryId = s.Category.Id,
                    CategoryName = s.Category.Name
                }).ToList();

                this.logger.LogInformation("GetSkills completed successfully. Found {SkillCount} skills", skills.Count);
                return Results.Ok(skills);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while fetching skills");
                return Results.InternalServerError();
            }
        }

        [HttpGet("{id:guid}")]
        public IResult GetSkill([FromRoute] Guid id)
        {
            this.logger.LogInformation("GetSkill called with ID: {SkillId}", id);
            try
            {
                var skill = this.skillRepository.GetSkill(id);
                if (skill is null)
                {
                    this.logger.LogWarning("Skill not found with ID: {SkillId}", id);
                    return Results.NotFound($"Skill not found by id : {id}");
                }
                this.logger.LogInformation("GetSkill completed successfully for ID: {SkillId}, Name: {SkillName}", skill.Id, skill.Name);
                var skillDto = new SkillDetailDto()
                {
                    Id = skill.Id,
                    Name = skill.Name,
                    CategoryId = skill.Category.Id,
                    CategoryName = skill.Category.Name
                };
                return Results.Ok(skillDto);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while fetching skill with ID: {SkillId}", id);
                return Results.InternalServerError();
            }
        }

        [HttpPost]
        public async Task<IResult> CreateSkill([FromBody] CreateOrUpdateSkillDto skillDto)
        {
            this.logger.LogInformation("CreateSkill called with Name: {SkillName}", skillDto.Name);

            if (!ModelState.IsValid)
                return Results.BadRequest(ModelState);

            try
            {
                var skill = new Skill()
                {
                    Name = skillDto.Name,
                    CategoryId = skillDto.CategoryId
                };

                this.logger.LogDebug("Adding new skill to context: {SkillDetails}",
                new
                {
                    skillDto.Name,
                    skillDto.CategoryId
                }
            );
                this.skillRepository.CreateSkill(skill);
                this.logger.LogInformation("CreateSkill completed successfully for ID: {SkillId}, Name: {SkillName}", skill.Id, skill.Name);
                return Results.Created($"/api/skills/{skill.Id}", skill);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while creating skill with Name: {SkillName}", skillDto.Name);
                return Results.InternalServerError();
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IResult> UpdateSkill([FromRoute] Guid id, [FromBody] CreateOrUpdateSkillDto skillDto)
        {
            this.logger.LogInformation("UpdateSkill called with ID: {SkillId}", id);
            if (!ModelState.IsValid)
                return Results.BadRequest(ModelState);
            try
            {
                var existingSkill = this.skillRepository.GetSkill(id);
                if (existingSkill is null)
                {
                    this.logger.LogWarning("Skill not found with ID: {SkillId}", id);
                    return Results.NotFound($"Skill not found by id : {id}");
                }
                existingSkill.Name = skillDto.Name;
                existingSkill.CategoryId = skillDto.CategoryId;
                this.logger.LogDebug("Updating skill in context: {SkillDetails}",
                new
                {
                    id,
                    skillDto.Name,
                    skillDto.CategoryId
                }
            );
                this.skillRepository.UpdateSkill(existingSkill);
                this.logger.LogInformation("UpdateSkill completed successfully for ID: {SkillId}, Name: {SkillName}", existingSkill.Id, existingSkill.Name);
                return Results.Ok(existingSkill);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while updating skill with ID: {SkillId}", id);
                return Results.InternalServerError();
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IResult> DeleteSkill([FromRoute] Guid id)
        {
            this.logger.LogInformation("DeleteCustomer called for ID: {SkillId}", id);

            try
            {
                Skill skill = this.skillRepository.GetSkill(id);
                if (skill is null)
                {
                    this.logger.LogWarning("Cannot delete skill - not found with ID: {SkillId}", id);
                    return Results.NotFound($"Skill not found by id : {id}");
                }

                this.logger.LogInformation("Deleting skill: ID={SkillId}, Name='{SkillName}'",
                    skill.Id,
                    skill.Name);

                this.skillRepository.DeleteSkill(skill);

                this.logger.LogInformation("DeleteSkill completed successfully for ID: {SkillId}", id);
                return Results.NoContent();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while deleting skill with ID: {SkillId}", id);
                return Results.InternalServerError(e);
            }
        }
    }
}
