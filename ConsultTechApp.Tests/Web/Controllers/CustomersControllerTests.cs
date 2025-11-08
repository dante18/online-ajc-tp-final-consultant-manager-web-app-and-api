using ConsultTechApp.Web.Controllers;
using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Dtos.Application.Customers;
using ConsultTechApp.Web.Services.Dtos.Application.Mission;
using ConsultTechApp.Web.ViewsModels.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace ConsultTechApp.Tests.Web.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<IApiCustomersService> _customersServiceMock;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _customersServiceMock = new Mock<IApiCustomersService>();

        _controller = new CustomersController(_customersServiceMock.Object);

        // ⚠️ Since the controller uses TempData, a "fake" one is injected into it.
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _controller.TempData = tempData;
    }

    [Fact]
    public async Task Index_ShouldReturnView_WithCustomersList()
    {
        // Arrange
        _customersServiceMock
            .Setup(s => s.GetCustomersAsync())
            .ReturnsAsync(new List<CustomerDto>
            {
                    new() { Id = Guid.NewGuid(), CompanyName = "Acme", Industry = "IT", Address = "Paris" },
                    new() { Id = Guid.NewGuid(), CompanyName = "TechNova", Industry = "Informatique", Address = "Lyon" }
            });

        // Act
        var result = await _controller.Index();

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomersListViewModel>(view.Model);
        Assert.Equal(2, model.Items.Count);
        Assert.Contains(model.Items, c => c.CompanyName == "Acme");
    }

    [Fact]
    public async Task Details_ShouldReturnView_WhenCustomerExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customersServiceMock
            .Setup(s => s.GetCustomerAsync(id))
            .ReturnsAsync(new CustomerDetailDto
            {
                Id = id,
                CompanyName = "Acme",
                Industry = "IT",
                Address = "Paris",
                ContactName = "John Doe",
                ContactEmail = "john@acme.com",
                Missions = new List<MissionDto>
                {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = "Mission 1",
                            Description = "Test",
                            StartDate = DateTime.UtcNow,
                            EstimatedBudget = 1000
                        }
                }
            });

        // Act
        var result = await _controller.Details(id);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomersDetailsViewModel>(view.Model);
        Assert.Equal("Acme", model.CompanyName);
        Assert.Single(model.Missions);
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenCustomerIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customersServiceMock
            .Setup(s => s.GetCustomerAsync(id))
            .ReturnsAsync((CustomerDetailDto?)null);

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
        _customersServiceMock
            .Setup(s => s.GetCustomerAsync(id))
            .ThrowsAsync(new Exception("API down"));

        // Act
        var result = await _controller.Details(id);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CustomersController.Index), redirect.ActionName);
        Assert.Equal("error", _controller.TempData["messageType"]);
        Assert.NotNull(_controller.TempData["message"]);
    }

    [Fact]
    public void Create_Get_ShouldReturnView()
    {
        // Act
        var result = _controller.Create();

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<CustomersCreateViewModel>(view.Model);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnView_WhenModelStateInvalid()
    {
        // Arrange
        var vm = new CustomersCreateViewModel
        {
            CompanyName = "",
        };

        _controller.ModelState.AddModelError("CompanyName", "Required");

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(vm, view.Model);
        _customersServiceMock.Verify(s => s.CreateCustomerAsync(It.IsAny<CreateOrUpdateCustomerDto>()), Times.Never);
    }

    [Fact]
    public async Task Create_Post_ShouldRedirect_WhenValid()
    {
        // Arrange
        var vm = new CustomersCreateViewModel
        {
            CompanyName = "Acme",
            Industry = "IT",
            Address = "Paris",
            ContactEmail = "contact@acme.com",
            ContactName = "John Doe"
        };

        _customersServiceMock
            .Setup(s => s.CreateCustomerAsync(It.IsAny<CreateOrUpdateCustomerDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CustomersController.Index), redirect.ActionName);
        Assert.Equal("success", _controller.TempData["messageType"]);
        Assert.Equal("Customer creation successful!", _controller.TempData["message"]);
        _customersServiceMock.Verify(s => s.CreateCustomerAsync(It.IsAny<CreateOrUpdateCustomerDto>()), Times.Once);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customersServiceMock
            .Setup(s => s.GetCustomerAsync(id))
            .ReturnsAsync((CustomerDetailDto?)null);

        // Act
        var result = await _controller.Edit(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnView_WhenCustomerExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customersServiceMock
            .Setup(s => s.GetCustomerAsync(id))
            .ReturnsAsync(new CustomerDetailDto
            {
                Id = id,
                CompanyName = "Acme",
                Industry = "IT",
                Address = "Paris",
                ContactEmail = "contact@acme.com",
                ContactName = "John"
            });

        // Act
        var result = await _controller.Edit(id);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomersEditViewModel>(view.Model);
        Assert.Equal("Acme", model.CompanyName);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnView_WhenCustomerExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customersServiceMock
            .Setup(s => s.GetCustomerAsync(id))
            .ReturnsAsync(new CustomerDetailDto
            {
                Id = id,
                CompanyName = "Acme",
                Industry = "IT",
                Address = "Paris",
                ContactEmail = "contact@acme.com",
                ContactName = "John"
            });

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomersDeleteViewModel>(view.Model);
        Assert.Equal("Acme", model.CompanyName);
        _customersServiceMock.Verify(s => s.GetCustomerAsync(id), Times.Once);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customersServiceMock
            .Setup(s => s.GetCustomerAsync(id))
            .ReturnsAsync((CustomerDetailDto?)null);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _customersServiceMock.Verify(s => s.GetCustomerAsync(id), Times.Once);
    }

    [Fact]
    public async Task Delete_Get_ShouldRedirectToIndex_WhenException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customersServiceMock
            .Setup(s => s.GetCustomerAsync(id))
            .ThrowsAsync(new Exception("API down"));

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CustomersController.Index), redirect.ActionName);
        Assert.Equal("error", _controller.TempData["messageType"]);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldRedirectToIndex_AndCallService()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customersServiceMock
            .Setup(s => s.DeleteCustomerAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(id);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CustomersController.Index), redirect.ActionName);
        Assert.Equal("success", _controller.TempData["messageType"]);
        Assert.Equal("Customer delete successful!", _controller.TempData["message"]);
        _customersServiceMock.Verify(s => s.DeleteCustomerAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldRedirectToIndex_WhenServiceThrows()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customersServiceMock
            .Setup(s => s.DeleteCustomerAsync(id))
            .ThrowsAsync(new Exception("delete failed"));

        // Act
        var result = await _controller.DeleteConfirmed(id);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CustomersController.Index), redirect.ActionName);
        Assert.Equal("error", _controller.TempData["messageType"]);
        Assert.Equal("Customer delete failed!", _controller.TempData["message"]);
        _customersServiceMock.Verify(s => s.DeleteCustomerAsync(id), Times.Once);
    }
}
