using ConsultTechApp.Web.Services.Dtos.Application.Customers;
using ConsultTechApp.Web.Services.Services.Application;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace ConsultTechApp.Tests.Web.Services.Services;

    public class CustomersServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

    public CustomersServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
    }

    [Fact]
    public async Task GetCustomersAsync_ShouldReturnList_WhenApiReturns200()
    {
        // Arrange
        var customers = new List<CustomerDto>
            {
                new() { Id = Guid.NewGuid(), CompanyName = "Acme", Address = "Paris" },
                new() { Id = Guid.NewGuid(), CompanyName = "TechNova", Address = "Lyon" }
            };

        var json = JsonSerializer.Serialize(customers);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        // Act
        var result = await service.GetCustomersAsync();

        // Assert
        var list = result.ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal("Acme", list[0].CompanyName);
        Assert.Equal("http://localhost:5000/api/customers/", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetCustomerAsync_ShouldReturnCustomer_WhenApiReturns200()
    {
        // Arrange
        var id = Guid.NewGuid();
        var customer = new CustomerDetailDto
        {
            Id = id,
            CompanyName = "Acme",
            Address = "Paris",
            Industry = "IT",
            ContactEmail = "contact@acme.com",
            ContactName = "John Doe"
        };

        var json = JsonSerializer.Serialize(customer);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        // Act
        var result = await service.GetCustomerAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Acme", result!.CompanyName);
        Assert.IsType<CustomerDetailDto>(result);
        Assert.Equal($"http://localhost:5000/api/customers/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetCustomerAsync_ShouldThrow_WhenApiReturns404()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetCustomerAsync(id));
        Assert.Equal($"http://localhost:5000/api/customers/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task CreateCustomerAsync_ShouldPostCustomer_WhenApiReturns201()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Created);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        var newCustomer = new CreateOrUpdateCustomerDto
        {
            CompanyName = "NewCo",
            Industry = "IT",
            Address = "Paris",
            ContactEmail = "contact@newco.com",
            ContactName = "Alice"
        };

        // Act
        await service.CreateCustomerAsync(newCustomer);

        // Assert
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("http://localhost:5000/api/customers/", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task CreateCustomerAsync_ShouldThrow_WhenApiReturns400()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        var newCustomer = new CreateOrUpdateCustomerDto
        {
            CompanyName = "BadCo"
        };

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.CreateCustomerAsync(newCustomer));
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldPutCustomer_WhenApiReturns204()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        var updateCustomer = new CreateOrUpdateCustomerDto
        {
            CompanyName = "Updated Co",
            Industry = "Finance",
            Address = "Lyon",
            ContactEmail = "updated@co.com",
            ContactName = "Bob"
        };

        // Act
        await service.UpdateCustomerAsync(id, updateCustomer);

        // Assert
        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Equal($"http://localhost:5000/api/customers/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldThrow_WhenApiReturns400()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        var updateCustomer = new CreateOrUpdateCustomerDto
        {
            CompanyName = "" // mauvais modèle
        };

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.UpdateCustomerAsync(id, updateCustomer));
        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldThrow_WhenApiReturns404()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        var updateCustomer = new CreateOrUpdateCustomerDto
        {
            CompanyName = "Updated Co"
        };

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.UpdateCustomerAsync(id, updateCustomer));
        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
    }

    [Fact]
    public async Task DeleteCustomerAsync_ShouldSendDelete_WhenApiReturns204()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        // Act
        await service.DeleteCustomerAsync(id);

        // Assert
        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Equal($"http://localhost:5000/api/customers/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task DeleteCustomerAsync_ShouldThrow_WhenApiReturns404()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/customers/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Customers"))
            .Returns(client);

        var service = new CustomersService(_httpClientFactoryMock.Object);

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.DeleteCustomerAsync(id));
        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
    }
}
