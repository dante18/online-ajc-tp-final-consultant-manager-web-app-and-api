using ConsultTechApp.Api.Controllers;
using ConsultTechApp.Api.Dtos.Mission;
using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConsultTechApp.Tests.Api.Controllers;

public class MissionsControllerTests
{
    private readonly Mock<IMissionRepository> _missionRepoMock;
    private readonly Mock<ILogger<MissionsController>> _loggerMock;
    private readonly MissionsController _controller;

    public MissionsControllerTests()
    {
        _missionRepoMock = new Mock<IMissionRepository>();
        _loggerMock = new Mock<ILogger<MissionsController>>();
        _controller = new MissionsController(_loggerMock.Object, _missionRepoMock.Object);
    }

    [Fact]
    public void GetMissions_ShouldReturnOk_WhenMissionsExist()
    {
        // Arrange
        var missions = new List<Mission>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Migration Base de Données",
                    Description = "Migration du système SQL Server vers Azure SQL.",
                    StartDate = DateTime.UtcNow.AddDays(7),
                    EndDate = DateTime.UtcNow.AddMonths(2),
                    EstimatedBudget = 30000,
                    Customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        CompanyName = "Acme Corp",
                        Industry = "Finance",
                        Address = "1 Rue de la Bourse, Paris",
                        ContactName = "Jean Petit",
                        ContactEmail = "jean@acme.com"
                    },
                    Assignments = new List<MissionAssignment>()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Développement API REST",
                    Description = "Conception d'une API REST pour un portail client.",
                    StartDate = DateTime.UtcNow.AddDays(15),
                    EndDate = DateTime.UtcNow.AddMonths(3),
                    Customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        CompanyName = "TechNova",
                        Industry = "Informatique",
                        Address = "10 Avenue du Web, Lyon",
                        ContactName = "Sophie Lambert",
                        ContactEmail = "sophie@technova.com"
                    },
                    Assignments = new List<MissionAssignment>()
                }
            };

        _missionRepoMock.Setup(r => r.GetAllMissions()).Returns(missions);

        // Act
        var result = _controller.GetMissions();

        // Assert
        Assert.NotNull(result);
        var okResult = result as Ok<List<MissionDto>>;
        Assert.NotNull(okResult);
    }

    [Fact]
    public void GetMission_ShouldReturnOk_WhenMissionExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mission = new Mission
        {
            Id = id,
            Title = "Développement API REST",
            Description = "Conception d'une API REST pour un portail client.",
            StartDate = DateTime.UtcNow.AddDays(15),
            EndDate = DateTime.UtcNow.AddMonths(3),
            Customer = new Customer
            {
                Id = Guid.NewGuid(),
                CompanyName = "TechNova",
                Industry = "Informatique",
                Address = "10 Avenue du Web, Lyon",
                ContactName = "Sophie Lambert",
                ContactEmail = "sophie@technova.com"
            },
            Assignments = new List<MissionAssignment>()
        };

        _missionRepoMock.Setup(r => r.GetMission(id)).Returns(mission);

        // Act
        var result = _controller.GetMission(id);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Ok<MissionDto>>(result);
    }

    [Fact]
    public void GetMission_ShouldReturnNotFound_WhenMissionDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionRepoMock.Setup(r => r.GetMission(id)).Returns((Mission)null!);

        // Act
        var result = _controller.GetMission(id);

        // Assert
        Assert.IsType<NotFound<string>>(result);
    }

    [Fact]
    public void CreateMission_ShouldReturnCreated_WhenValid()
    {
        // Arrange
        var dto = new CreateOrUpdateMissionDto
        {
            Title = "Développement API REST FULL",
            Description = "Conception d'une API REST pour un portail client.",
            StartDate = DateTime.UtcNow.AddDays(15),
            EndDate = DateTime.UtcNow.AddMonths(3),
            CustomerId = Guid.NewGuid(),
            EstimatedBudget = 500
        };

        _missionRepoMock
            .Setup(r => r.CreateMission(It.IsAny<Mission>()))
            .Callback<Mission>(m =>
            {
                // on simule que la BD lui a donné un Id
                m.Id = Guid.NewGuid();
            });

        _missionRepoMock
            .Setup(r => r.GetMission(It.IsAny<Guid>()))
            .Returns<Guid>(id => new Mission
            {
                Id = id,
                Title = dto.Title,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                EstimatedBudget = dto.EstimatedBudget,
                Customer = new Customer
                {
                    Id = dto.CustomerId,
                    CompanyName = "Fake Customer",
                    Address = "Somewhere",
                    ContactEmail = "contact@fake.com",
                    Industry = "Fake"
                },
                Assignments = new List<MissionAssignment>()
            });

        // Act
        var result = _controller.CreateMission(dto);

        // Assert
        var created = Assert.IsType<Created<MissionDto>>(result);
        Assert.Equal(dto.Title, created.Value.Title);
        Assert.Equal("Fake Customer", created.Value.Customer.CompanyName);
        _missionRepoMock.Verify(r => r.CreateMission(It.IsAny<Mission>()), Times.Once);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(-10, false)]
    [InlineData(0.5, false)]
    public void CreateMission_ShouldReturnBadRequest_WhenEstimatedBudgetNotValid(decimal estimatedBudget, bool shouldBeValid)
    {
        // Arrange
        var dto = new CreateOrUpdateMissionDto
        {
            Title = "Mission",
            Description = "Test",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            CustomerId = Guid.NewGuid(),
            EstimatedBudget = estimatedBudget
        };

        if (!shouldBeValid)
            _controller.ModelState.AddModelError(nameof(dto.EstimatedBudget), "Budget must be >= 1");

        // Act
        var result = _controller.CreateMission(dto);

        // Assert
        Assert.IsType<BadRequest<Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary>>(result);
    }

    [Theory]
    [InlineData("2025-01-01", "2024-12-02", false)]
    [InlineData("2025-01-01", "2025-01-01", false)]
    [InlineData("2025-01-02", "2025-01-01", false)]
    public void CreateMission_ShouldReturnBadRequest_WhenStartDateNotValid(string startDate, string endDate, bool expectedValid)
    {
        // Arrange
        var dto = new CreateOrUpdateMissionDto
        {
            Title = "Test mission",
            Description = "Validation des dates",
            StartDate = DateTime.Parse(startDate),
            EndDate = DateTime.Parse(endDate),
            EstimatedBudget = 1000m,
            CustomerId = Guid.NewGuid()
        };

        if (!expectedValid)
            _controller.ModelState.AddModelError(nameof(dto.EstimatedBudget), "The end date must be later than the start date.");

        // Act
        var result = _controller.CreateMission(dto);

        // Assert
        Assert.IsType<BadRequest<Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary>>(result);
    }

    [Fact]
    public void UpdateMissionAsync_ShouldReturnNoContent_WhenMissionExists()
    {
        var id = Guid.NewGuid();
        var existing = new Mission
        {
            Id = id,
            Title = "Old",
            CustomerId = Guid.NewGuid()
        };

        var dto = new CreateOrUpdateMissionDto
        {
            Title = "Updated title",
            Description = "New desc",
            EstimatedBudget = 999
        };

        _missionRepoMock
            .Setup(r => r.GetMission(id))
            .Returns(existing);

        var result = _controller.UpdateMissionAsync(id, dto);

        Assert.IsType<NoContent>(result);
        _missionRepoMock.Verify(r => r.UpdateMission(It.Is<Mission>(m => m.Title == "Updated title")), Times.Once);
    }

    [Fact]
    public void UpdateMissionAsync_ShouldReturnNotFound_WhenMissionDoesNotExist()
    {
        var id = Guid.NewGuid();
        var dto = new CreateOrUpdateMissionDto
        {
            Title = "Updated title"
        };

        _missionRepoMock
            .Setup(r => r.GetMission(id))
            .Returns((Mission)null!);

        var result = _controller.UpdateMissionAsync(id, dto);

        var notFound = Assert.IsType<NotFound<string>>(result);
        Assert.Contains("Mission not found", notFound.Value);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(-10, false)]
    [InlineData(0.5, false)]
    public void UpdateMission_ShouldReturnBadRequest_WhenEstimatedBudgetNotValid(decimal estimatedBudget, bool shouldBeValid)
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CreateOrUpdateMissionDto
        {
            Title = "Mission",
            Description = "Test",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            CustomerId = Guid.NewGuid(),
            EstimatedBudget = estimatedBudget
        };

        if (!shouldBeValid)
            _controller.ModelState.AddModelError(nameof(dto.EstimatedBudget), "Budget must be >= 1");

        // Act
        var result = _controller.UpdateMissionAsync(id, dto);

        // Assert
        Assert.IsType<BadRequest<ModelStateDictionary>>(result);
    }

    [Theory]
    [InlineData("2025-01-01", "2024-12-02", false)]
    [InlineData("2025-01-01", "2025-01-01", false)]
    [InlineData("2025-01-02", "2025-01-01", false)]
    public void UpdateMission_ShouldReturnBadRequest_WhenStartDateNotValid(string startDate, string endDate, bool expectedValid)
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CreateOrUpdateMissionDto
        {
            Title = "Test mission",
            Description = "Validation des dates",
            StartDate = DateTime.Parse(startDate),
            EndDate = DateTime.Parse(endDate),
            EstimatedBudget = 1000m,
            CustomerId = Guid.NewGuid()
        };

        if (!expectedValid)
            _controller.ModelState.AddModelError(nameof(dto.EstimatedBudget), "The end date must be later than the start date.");

        // Act
        var result = _controller.UpdateMissionAsync(id, dto);

        // Assert
        Assert.IsType<BadRequest<ModelStateDictionary>>(result);
    }

    [Fact]
    public void DeleteMission_ShouldReturnNoContent_WhenMissionDeleted()
    {
        var id = Guid.NewGuid();
        var mission = new Mission
        {
            Id = id,
            Title = "To delete"
        };

        _missionRepoMock
            .Setup(r => r.GetMission(id))
            .Returns(mission);

        var result = _controller.DeleteMission(id);

        Assert.IsType<NoContent>(result);
        _missionRepoMock.Verify(r => r.DeleteMission(mission), Times.Once);
    }

    [Fact]
    public void DeleteMission_ShouldReturnNotFound_WhenMissionDoesNotExist()
    {
        var id = Guid.NewGuid();
        _missionRepoMock
            .Setup(r => r.GetMission(id))
            .Returns((Mission)null!);

        var result = _controller.DeleteMission(id);

        Assert.IsType<NotFound<string>>(result);
    }
}
