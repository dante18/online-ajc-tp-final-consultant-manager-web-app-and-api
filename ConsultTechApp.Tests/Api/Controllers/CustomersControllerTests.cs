using ConsultTechApp.Api.Controllers;
using ConsultTechApp.Api.Dtos.Customer;
using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConsultTechApp.Tests.Api.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerRepository> _customerRepoMock;
    private readonly Mock<ILogger<CustomersController>> _loggerMock;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _customerRepoMock = new Mock<ICustomerRepository>();
        _loggerMock = new Mock<ILogger<CustomersController>>();
        _controller = new CustomersController(_loggerMock.Object, _customerRepoMock.Object);
    }

    [Fact]
    public void GetCustomers_ShouldReturnOk_WhenCustomersExist()
    {
        // Arrange
        var customers = new List<Customer>
            {
                new() { Id = Guid.NewGuid(), CompanyName = "Acme", Industry = "Tech", Address = "Paris", ContactName = "John", ContactEmail = "john@acme.com" },
                new() { Id = Guid.NewGuid(), CompanyName = "Globex", Industry = "Finance", Address = "Lyon", ContactName = "Jane", ContactEmail = "jane@globex.com" }
            };

        _customerRepoMock.Setup(r => r.GetAllCustomers()).Returns(customers);

        // Act
        var result = _controller.GetCustomers();

        // Assert
        Assert.NotNull(result);
        var okResult = result as Ok<List<CustomerDto>>;
        Assert.NotNull(okResult);
    }

    [Fact]
    public async Task GetCustomerAsync_ShouldReturnOk_WhenCustomerExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var customer = new Customer
        {
            Id = id,
            CompanyName = "Acme Corp",
            Industry = "Finance",
            Address = "Paris",
            ContactName = "John Doe",
            ContactEmail = "john@acme.com",
            Missions = new List<Mission>()
        };

        _customerRepoMock.Setup(r => r.GetCustomer(id)).Returns(customer);

        // Act
        var result = await _controller.GetCustomerAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Ok<CustomerDetailDto>>(result);
    }

    [Fact]
    public async Task GetCustomerAsync_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _customerRepoMock.Setup(r => r.GetCustomer(id)).Returns((Customer)null!);

        // Act
        var result = await _controller.GetCustomerAsync(id);

        // Assert
        Assert.IsType<NotFound<string>>(result);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnCreated_WhenValid()
    {
        // Arrange
        var dto = new CreateOrUpdateCustomerDto
        {
            CompanyName = "Acme",
            Industry = "Tech",
            Address = "Paris",
            ContactName = "John Doe",
            ContactEmail = "john@acme.com"
        };

        _customerRepoMock.Setup(r => r.CreateCustomer(It.IsAny<Customer>()));

        // Act
        var result = await _controller.CreateCustomer(dto);

        // Assert
        Assert.IsType<Created<CustomerDto>>(result);
        _customerRepoMock.Verify(r => r.CreateCustomer(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var dto = new CreateOrUpdateCustomerDto
        {
            CompanyName = "",
            Address = "Paris"
        };

        _controller.ModelState.AddModelError("CompanyName", "The CompanyName field is required.");

        // Act
        var result = await _controller.CreateCustomer(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequest<ModelStateDictionary>>(result);
        Assert.True(badRequest.Value.ContainsKey("CompanyName"));
        _customerRepoMock.Verify(r => r.CreateCustomer(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldReturnNoContent_WhenCustomerExists()
    {
        var id = Guid.NewGuid();
        var customer = new Customer
        {
            Id = id,
            CompanyName = "OldName"
        };
        var dto = new CreateOrUpdateCustomerDto { CompanyName = "NewName" };

        _customerRepoMock.Setup(r => r.GetCustomer(id)).Returns(customer);

        var result = await _controller.UpdateCustomerAsync(id, dto);

        Assert.IsType<NoContent>(result);
        _customerRepoMock.Verify(r => r.UpdateCustomer(It.Is<Customer>(c => c.CompanyName == "NewName")), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        var id = Guid.NewGuid();
        var dto = new CreateOrUpdateCustomerDto { CompanyName = "NewName" };

        _customerRepoMock.Setup(r => r.GetCustomer(id)).Returns((Customer)null!);

        var result = await _controller.UpdateCustomerAsync(id, dto);

        Assert.IsType<NotFound<string>>(result);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CreateOrUpdateCustomerDto
        {
            CompanyName = "",
            Address = "Paris"
        };

        _controller.ModelState.AddModelError("CompanyName", "The CompanyName field is required.");

        // Act
        var result = await _controller.UpdateCustomerAsync(id, dto);

        // Assert
        var badRequest = Assert.IsType<BadRequest<ModelStateDictionary>>(result);
        Assert.True(badRequest.Value.ContainsKey("CompanyName"));
        _customerRepoMock.Verify(r => r.CreateCustomer(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnNoContent_WhenCustomerDeleted()
    {
        var id = Guid.NewGuid();
        var customer = new Customer { Id = id, CompanyName = "Acme", Missions = new List<Mission>() };
        _customerRepoMock.Setup(r => r.GetCustomer(id)).Returns(customer);

        var result = await _controller.DeleteCustomer(id);

        Assert.IsType<NoContent>(result);
        _customerRepoMock.Verify(r => r.DeleteCustomer(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnBadRequest_WhenCustomerHasMissions()
    {
        var id = Guid.NewGuid();
        var customer = new Customer
        {
            Id = id,
            CompanyName = "Acme",
            Missions = new List<Mission> { new Mission { Title = "Test" } }
        };
        _customerRepoMock.Setup(r => r.GetCustomer(id)).Returns(customer);

        var result = await _controller.DeleteCustomer(id);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        var id = Guid.NewGuid();
        _customerRepoMock.Setup(r => r.GetCustomer(id)).Returns((Customer)null!);

        var result = await _controller.DeleteCustomer(id);

        Assert.IsType<NotFound<string>>(result);
    }
}
