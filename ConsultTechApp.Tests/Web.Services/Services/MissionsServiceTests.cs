using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using ConsultTechApp.Web.Services.Dtos.Application.Consultant;
using ConsultTechApp.Web.Services.Dtos.Application.Mission;
using ConsultTechApp.Web.Services.Services.Application;
using Moq;

namespace ConsultTechApp.Tests.Web.Services.Services;

public class MissionsServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

    public MissionsServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
    }

    [Fact]
    public async Task GetMissionsAsync_ShouldReturnList_WhenApiReturns200()
    {
        // Arrange
        var missions = new List<MissionDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Mission 1",
                    Description = "Test",
                    StartDate = DateTime.UtcNow,
                    EstimatedBudget = 1000,
                    Customer = new ConsultTechApp.Web.Services.Dtos.Application.Customers.CustomerDto
                    {
                        Id = Guid.NewGuid(),
                        CompanyName = "Acme"
                    },
                    Consultants = new List<ConsultantDto>()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Mission 2",
                    Description = "Test 2",
                    StartDate = DateTime.UtcNow,
                    EstimatedBudget = 2000,
                    Customer = new ConsultTechApp.Web.Services.Dtos.Application.Customers.CustomerDto
                    {
                        Id = Guid.NewGuid(),
                        CompanyName = "TechNova"
                    },
                    Consultants = new List<ConsultantDto>()
                }
            };

        var json = JsonSerializer.Serialize(missions);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        // Act
        var result = await service.GetMissionsAsync();

        // Assert
        var list = result.ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal("Mission 1", list[0].Title);
        Assert.Equal("http://localhost:5000/api/missions/", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetMissionAsync_ShouldReturnMission_WhenApiReturns200()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mission = new MissionDto
        {
            Id = id,
            Title = "Audit SI",
            Description = "desc",
            StartDate = DateTime.UtcNow,
            EstimatedBudget = 1500,
            Customer = new ConsultTechApp.Web.Services.Dtos.Application.Customers.CustomerDto
            {
                Id = Guid.NewGuid(),
                CompanyName = "Acme"
            },
            Consultants = new List<ConsultantDto>()
        };

        var json = JsonSerializer.Serialize(mission);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        // Act
        var result = await service.GetMissionAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Audit SI", result!.Title);
        Assert.IsType<MissionDto>(result);
        Assert.Equal($"http://localhost:5000/api/missions/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetMissionAsync_ShouldThrow_WhenApiReturns404()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetMissionAsync(id));
        Assert.Equal($"http://localhost:5000/api/missions/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task CreateMissionAsync_ShouldPostMission_WhenApiReturns201()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Created);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        var newMission = new CreateOrUpdateMissionDto
        {
            Title = "New Mission",
            Description = "desc",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3),
            EstimatedBudget = 1000,
            CustomerId = Guid.NewGuid()
        };

        // Act
        await service.CreateMissionAsync(newMission);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("http://localhost:5000/api/missions/", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task CreateMissionAsync_ShouldThrow_WhenApiReturns400()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        var newMission = new CreateOrUpdateMissionDto
        {
            Title = ""
        };

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.CreateMissionAsync(newMission));
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
    }

    [Fact]
    public async Task UpdateMissionAsync_ShouldPutMission_WhenApiReturns204()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        var updateMission = new CreateOrUpdateMissionDto
        {
            Title = "Updated Mission",
            Description = "desc updated",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            EstimatedBudget = 2000,
            CustomerId = Guid.NewGuid()
        };

        // Act
        await service.UpdateMissionAsync(id, updateMission);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Equal($"http://localhost:5000/api/missions/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task UpdateMissionAsync_ShouldThrow_WhenApiReturns400()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        var updateMission = new CreateOrUpdateMissionDto
        {
            Title = ""
        };

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.UpdateMissionAsync(id, updateMission));
        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
    }

    [Fact]
    public async Task UpdateMissionAsync_ShouldThrow_WhenApiReturns404()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        var updateMission = new CreateOrUpdateMissionDto
        {
            Title = "Updated"
        };

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.UpdateMissionAsync(id, updateMission));
        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
    }

    [Fact]
    public async Task DeleteMissionAsync_ShouldSendDelete_WhenApiReturns204()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        // Act
        await service.DeleteMissionAsync(id);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Equal($"http://localhost:5000/api/missions/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task DeleteMissionAsync_ShouldThrow_WhenApiReturns404()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/api/missions/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("Missions"))
            .Returns(client);

        var service = new MissionsService(_httpClientFactoryMock.Object);

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.DeleteMissionAsync(id));
        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
    }
}
