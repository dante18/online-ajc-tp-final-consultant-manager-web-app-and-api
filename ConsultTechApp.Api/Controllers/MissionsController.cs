using ConsultTechApp.Api.Dtos.Consultant;
using ConsultTechApp.Api.Dtos.Customer;
using ConsultTechApp.Api.Dtos.Mission;
using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConsultTechApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MissionsController : ControllerBase
{
    private readonly ILogger<MissionsController> logger;

    private readonly IMissionRepository missionService;

    public MissionsController(ILogger<MissionsController> logger, IMissionRepository missionService)
    {
        this.logger = logger;
        this.missionService = missionService;
        this.logger.LogInformation("MissionsController initialized");
    }

    [HttpGet]
    public IResult GetMissions()
    {
        this.logger.LogInformation("GetMissions called");

        try
        {
            List<MissionDto> missions = this.missionService.GetAllMissions().Select(m => new MissionDto()
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                EstimatedBudget = m.EstimatedBudget,
                Customer = new CustomerDto()
                {
                    Address = m.Customer.Address,
                    CompanyName = m.Customer.CompanyName,
                    ContactEmail = m.Customer.ContactEmail,
                    Id = m.Customer.Id,
                    Industry = m.Customer.Industry
                },
                Consultants = m.Assignments?.Select(c => new ConsultantDto()
                { 
                    Id = c.Consultant.Id,
                    FirstName = c.Consultant.FirstName,
                    LastName = c.Consultant.LastName,
                    Email = c.Consultant.Email,
                    HireDate = c.Consultant.HireDate,
                    IsActive = c.Consultant.IsActive
                }).ToList()
            }).ToList();

            this.logger.LogInformation("GetMissions completed successfully. Found {MissionCount} missions", missions.Count);

            return Results.Ok(missions);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while fetching missions");

            return Results.InternalServerError();
        }
    }

    [HttpGet("{id:guid}")]
    public IResult GetMission([FromRoute] Guid id)
    {
        this.logger.LogInformation("GetMission called with ID: {MissionId}", id);

        try
        {
            Mission mission = this.missionService.GetMission(id);
            if (mission is null)
            {
                this.logger.LogWarning("Mission not found with ID: {MissionId}", id);
                return Results.NotFound($"Mission not found by id : {id}");
            }

            this.logger.LogInformation("GetMission completed successfully for ID: {MissionId}, Name: {Title}",
                id,
                mission.Title);

            MissionDto missionDto = new MissionDto()
            {
                Id = mission.Id,
                Title = mission.Title,
                Description = mission.Description,
                StartDate = mission.StartDate,
                EndDate = mission.EndDate,
                EstimatedBudget = mission.EstimatedBudget,
                Customer = new CustomerDto()
                {
                    Address = mission.Customer.Address,
                    CompanyName = mission.Customer.CompanyName,
                    ContactEmail = mission.Customer.ContactEmail,
                    Id = mission.Customer.Id,
                    Industry = mission.Customer.Industry
                },
                Consultants = mission.Assignments?.Select(c => new ConsultantDto()
                {
                    Id = c.Consultant.Id,
                    FirstName = c.Consultant.FirstName,
                    LastName = c.Consultant.LastName,
                    Email = c.Consultant.Email,
                    HireDate = c.Consultant.HireDate,
                    IsActive = c.Consultant.IsActive
                }).ToList()
            };

            return Results.Ok(missionDto);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while fetching mission with ID: {MissionId}", id);
            return Results.InternalServerError(e);
        }
    }

    [HttpPost()]
    public IResult CreateMission([FromBody] CreateOrUpdateMissionDto dto)
    {
        this.logger.LogInformation(
            "CreateMission called with Title: {Title}, StartDate: {StartDate}, EndDate: {EndDate}, EstimatedBudget: {EstimatedBudget}, CustomerId: {CustomerId}",
            dto.Title, dto.StartDate, dto.EndDate, dto.EstimatedBudget, dto.CustomerId);

        if (!ModelState.IsValid)
            return Results.BadRequest(ModelState);

        try
        {
            Mission mission = new Mission()
            {
                Title = dto.Title,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                EstimatedBudget = dto.EstimatedBudget,
                CustomerId = dto.CustomerId
            };

            this.logger.LogDebug("Adding new mission to context: {MissionDetails}",
                new
                {
                    dto.Title,
                    dto.StartDate,
                    dto.EndDate,
                    dto.EstimatedBudget,
                    dto.CustomerId
                }
            );

            this.missionService.CreateMission(mission);

            var existingMission = this.missionService.GetMission(mission.Id);

            this.logger.LogInformation("CreateMission completed successfully. New mission ID: {MissionId}, Name: {Title}",
                mission.Id,
                mission.Title);

            MissionDto missionDto = new MissionDto()
            {
                Id = existingMission.Id,
                Title = existingMission.Title,
                Description = existingMission.Description,
                StartDate = existingMission.StartDate,
                EndDate = existingMission.EndDate,
                EstimatedBudget = existingMission.EstimatedBudget,
                Customer = new CustomerDto()
                {
                    Address = existingMission.Customer.Address,
                    CompanyName = existingMission.Customer.CompanyName,
                    ContactEmail = existingMission.Customer.ContactEmail,
                    Id = existingMission.Customer.Id,
                    Industry = existingMission.Customer.Industry
                },
                Consultants = existingMission.Assignments?.Select(c => new ConsultantDto()
                {
                    Id = c.Consultant.Id,
                    FirstName = c.Consultant.FirstName,
                    LastName = c.Consultant.LastName,
                    Email = c.Consultant.Email,
                    HireDate = c.Consultant.HireDate,
                    IsActive = c.Consultant.IsActive
                }).ToList()
            };

            return Results.Created($"/api/missions/{mission.Id}", missionDto);
        }
        catch (Exception e)
        {
            this.logger.LogError(e,
                "An error occurred while creating mission with Title: {Title}",
                dto.Title);

            return Results.InternalServerError(e);
        }
    }

    [HttpPut("{id:guid}")]
    public IResult UpdateMissionAsync([FromRoute] Guid id, [FromBody] CreateOrUpdateMissionDto dto)
    {
        this.logger.LogInformation("UpdateMission called for ID: {MissionId} with data: {UpdateData}",
            id,
            new
            {
                dto.Title,
                dto.Description,
                dto.StartDate,
                dto.EndDate,
                dto.EstimatedBudget
            });

        if (!ModelState.IsValid)
            return Results.BadRequest(ModelState);

        try
        {
            Mission mission = this.missionService.GetMission(id);
            if (mission is null)
            {
                this.logger.LogWarning("Cannot update mission - not found with ID: {MissionId}", id);
                return Results.NotFound($"Mission not found by id : {id}");
            }

            this.logger.LogDebug("Found existing mission: {MissionName}",
                mission.Title);

            if (dto.Title != null) mission.Title = dto.Title;
            if (dto.Description != null) mission.Description = dto.Description;
            if (dto.StartDate != null) mission.StartDate = dto.StartDate;
            if (dto.EndDate != null) mission.EndDate = dto.EndDate;
            if (dto.EstimatedBudget != null) mission.EstimatedBudget = dto.EstimatedBudget;
            if (dto.CustomerId != null) mission.CustomerId = dto.CustomerId;

            this.missionService.UpdateMission(mission);

            this.logger.LogInformation("UpdateMission completed successfully for ID: {MissionId}", id);

            return Results.NoContent();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while updating mission with ID: {MissionId}", id);
            return Results.InternalServerError(e);
        }
    }

    [HttpDelete("{id:guid}")]
    public IResult DeleteMission([FromRoute] Guid id)
    {
        this.logger.LogInformation("DeleteMission called for ID: {MissionId}", id);

        try
        {
            Mission mission = this.missionService.GetMission(id);
            if (mission is null)
            {
                this.logger.LogWarning("Cannot delete mission - not found with ID: {MissionId}", id);
                return Results.NotFound($"Mission not found by id : {id}");
            }

            this.logger.LogInformation("Deleting mission: ID={MissionId}, Title='{Title}'",
                mission.Id,
                mission.Title);

            this.missionService.DeleteMission(mission);

            this.logger.LogInformation("DeleteMission completed successfully for ID: {MissionId}", id);
            return Results.NoContent();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while deleting mission with ID: {MissionId}", id);
            return Results.InternalServerError(e);
        }
    }
}
