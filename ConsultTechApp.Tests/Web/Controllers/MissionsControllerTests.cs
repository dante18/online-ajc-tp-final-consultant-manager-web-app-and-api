using ConsultTechApp.Web.Controllers;
using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Dtos.Application.Consultant;
using ConsultTechApp.Web.Services.Dtos.Application.Customers;
using ConsultTechApp.Web.Services.Dtos.Application.Mission;
using ConsultTechApp.Web.ViewsModels.Missions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace ConsultTechApp.Tests.Web.Controllers;

public class MissionsControllerTests
{
    private readonly Mock<IApiMissionsService> _missionsServiceMock;
    private readonly Mock<IApiCustomersService> _customersServiceMock;
    private readonly MissionsController _controller;

    public MissionsControllerTests()
    {
        _missionsServiceMock = new Mock<IApiMissionsService>();
        _customersServiceMock = new Mock<IApiCustomersService>();

        _controller = new MissionsController(_missionsServiceMock.Object, _customersServiceMock.Object);

        // Since the controller uses TempData, a "fake" one is injected into it.
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _controller.TempData = tempData;
    }

    [Fact]
    public async Task Index_ShouldReturnView_WithMissionsList()
    {
        // Arrange
        _missionsServiceMock
            .Setup(s => s.GetMissionsAsync())
            .ReturnsAsync(new List<MissionDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Mission 1",
                    Description = "Desc",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5),
                    EstimatedBudget = 1000,
                    Customer = new CustomerDto
                    {
                        Id = Guid.NewGuid(),
                        CompanyName = "Acme"
                    },
                    Consultants = new List<ConsultantDto>
                    {
                        new() { FirstName = "Alice", LastName = "Durand" }
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Mission 2",
                    Description = "Desc 2",
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(10),
                    EstimatedBudget = 2000,
                    Customer = new CustomerDto
                    {
                        Id = Guid.NewGuid(),
                        CompanyName = "TechNova"
                    },
                    Consultants = new List<ConsultantDto>()
                }
            });

        // Act
        var result = await _controller.Index();

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<MissionsListViewModel>(view.Model);
        Assert.Equal(2, model.Items.Count);
        Assert.Equal("Mission 1", model.Items[0].Title);
        Assert.Equal("Acme", model.Items[0].Customer);
    }

    [Fact]
    public async Task Details_ShouldReturnView_WhenMissionExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionsServiceMock
            .Setup(s => s.GetMissionAsync(id))
            .ReturnsAsync(new MissionDto
            {
                Id = id,
                Title = "Audit infra",
                Description = "Test",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(3),
                EstimatedBudget = 1500,
                Customer = new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Acme",
                    Address = "Paris",
                    ContactEmail = "contact@acme.com",
                    ContactName = "John",
                    Industry = "IT"
                },
                Consultants = new List<ConsultantDto>
                {
                    new() { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Durand", Email = "a@a.a", IsActive = true, HireDate = DateTime.UtcNow }
                }
            });

        // Act
        var result = await _controller.Details(id);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<MissionsDetailsViewModel>(view.Model);
        Assert.Equal("Audit infra", model.Title);
        Assert.Equal("Acme", model.Customer.CompanyName);
        Assert.Single(model.Consultants);
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenMissionIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionsServiceMock
            .Setup(s => s.GetMissionAsync(id))
            .ReturnsAsync((MissionDto?)null);

        // Act
        var result = await _controller.Details(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ShouldRedirectToIndex_WithTempData_WhenException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionsServiceMock
            .Setup(s => s.GetMissionAsync(id))
            .ThrowsAsync(new Exception("API down"));

        // Act
        var result = await _controller.Details(id);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MissionsController.Index), redirect.ActionName);
        Assert.Equal("error", _controller.TempData["messageType"]);
        Assert.NotNull(_controller.TempData["message"]);
    }

    [Fact]
    public async Task Create_Get_ShouldReturnView()
    {
        // Arrange
        _customersServiceMock
            .Setup(s => s.GetCustomersAsync())
            .ReturnsAsync(new List<CustomerDto>
            {
                new() { Id = Guid.NewGuid(), CompanyName = "Acme" },
                new() { Id = Guid.NewGuid(), CompanyName = "TechNova" }
            });

        // Act
        var result = await _controller.Create();   // ✅ on attend l’action

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<MissionsCreateViewModel>(view.Model);
        Assert.Equal(2, model.CustomerList.Count);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnView_WhenModelStateInvalid()
    {
        // Arrange
        var vm = new MissionsCreateViewModel
        {
            Title = "", // invalide
        };
        _controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(vm, view.Model);
        _missionsServiceMock.Verify(s => s.CreateMissionAsync(It.IsAny<CreateOrUpdateMissionDto>()), Times.Never);
    }

    [Fact]
    public async Task Create_Post_ShouldRedirect_WhenValid()
    {
        // Arrange
        var vm = new MissionsCreateViewModel
        {
            Title = "New mission",
            Description = "Desc",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            EstimatedBudget = 1000,
            CustomerId = Guid.NewGuid()
        };

        _missionsServiceMock
            .Setup(s => s.CreateMissionAsync(It.IsAny<CreateOrUpdateMissionDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MissionsController.Index), redirect.ActionName);
        Assert.Equal("success", _controller.TempData["messageType"]);
        Assert.Equal("Mission creation successful!", _controller.TempData["message"]);
        _missionsServiceMock.Verify(s => s.CreateMissionAsync(It.IsAny<CreateOrUpdateMissionDto>()), Times.Once);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnView_WhenMissionExists()
    {
        // Arrange
        var id = Guid.NewGuid();

        _missionsServiceMock
            .Setup(s => s.GetMissionAsync(id))
            .ReturnsAsync(new MissionDto
            {
                Id = id,
                Title = "Audit infra",
                Description = "test",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(2),
                EstimatedBudget = 2000,
                Customer = new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Acme"
                }
            });

        _customersServiceMock
            .Setup(s => s.GetCustomersAsync())
            .ReturnsAsync(new List<CustomerDto>
            {
                    new() { Id = Guid.NewGuid(), CompanyName = "Acme" },
                    new() { Id = Guid.NewGuid(), CompanyName = "TechNova" }
            });

        // Act
        var result = await _controller.Edit(id);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<MissionsEditViewModel>(view.Model);
        Assert.Equal("Audit infra", model.Title);
        Assert.True(model.CustomerList.Count >= 2);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnNotFound_WhenMissionDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionsServiceMock
            .Setup(s => s.GetMissionAsync(id))
            .ReturnsAsync((MissionDto?)null);

        // Act
        var result = await _controller.Edit(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnView_WhenModelInvalid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var vm = new MissionsEditViewModel
        {
            Id = id,
            Title = ""
        };
        _controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await _controller.Edit(id, vm);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(vm, view.Model);
        _missionsServiceMock.Verify(s => s.UpdateMissionAsync(It.IsAny<Guid>(), It.IsAny<CreateOrUpdateMissionDto>()), Times.Never);
    }

    [Fact]
    public async Task Edit_Post_ShouldRedirect_WhenValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var vm = new MissionsEditViewModel
        {
            Id = id,
            Title = "Updated",
            Description = "desc",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            EstimatedBudget = 1500,
            CustomerId = Guid.NewGuid()
        };

        _missionsServiceMock
            .Setup(s => s.UpdateMissionAsync(id, It.IsAny<CreateOrUpdateMissionDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Edit(id, vm);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MissionsController.Index), redirect.ActionName);
        Assert.Equal("success", _controller.TempData["messageType"]);
        Assert.Equal("Mission update successful!", _controller.TempData["message"]);
        _missionsServiceMock.Verify(s => s.UpdateMissionAsync(id, It.IsAny<CreateOrUpdateMissionDto>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnView_WhenMissionExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionsServiceMock
            .Setup(s => s.GetMissionAsync(id))
            .ReturnsAsync(new MissionDto
            {
                Id = id,
                Title = "Audit infra",
                Description = "Test",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(3),
                EstimatedBudget = 1500,
                Customer = new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Acme",
                    Address = "Paris",
                    ContactEmail = "contact@acme.com",
                    ContactName = "John",
                    Industry = "IT"
                },
                Consultants = new List<ConsultantDto>
                {
                    new() { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Durand", Email = "a@a.a", IsActive = true, HireDate = DateTime.UtcNow }
                }
            });

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<MissionsDeleteViewModel>(view.Model);
        Assert.Equal("Audit infra", model.Title);
        _missionsServiceMock.Verify(s => s.GetMissionAsync(id), Times.Once);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnNotFound_WhenMissionDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionsServiceMock
            .Setup(s => s.GetMissionAsync(id))
            .ReturnsAsync((MissionDto?)null);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _missionsServiceMock.Verify(s => s.GetMissionAsync(id), Times.Once);
    }

    [Fact]
    public async Task Delete_Get_ShouldRedirectToIndex_WhenException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionsServiceMock
            .Setup(s => s.GetMissionAsync(id))
            .ThrowsAsync(new Exception("API down"));

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MissionsController.Index), redirect.ActionName);
        Assert.Equal("error", _controller.TempData["messageType"]);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldRedirectToIndex_AndCallService()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionsServiceMock
            .Setup(s => s.DeleteMissionAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(id);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MissionsController.Index), redirect.ActionName);
        Assert.Equal("success", _controller.TempData["messageType"]);
        Assert.Equal("Mission delete successful!", _controller.TempData["message"]);
        _missionsServiceMock.Verify(s => s.DeleteMissionAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldRedirectToIndex_WhenServiceThrows()
    {
        // Arrange
        var id = Guid.NewGuid();
        _missionsServiceMock
            .Setup(s => s.DeleteMissionAsync(id))
            .ThrowsAsync(new Exception("delete failed"));

        // Act
        var result = await _controller.DeleteConfirmed(id);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MissionsController.Index), redirect.ActionName);
        Assert.Equal("error", _controller.TempData["messageType"]);
        Assert.Equal("Mission delete failed!", _controller.TempData["message"]);
        _missionsServiceMock.Verify(s => s.DeleteMissionAsync(id), Times.Once);
    }
}
