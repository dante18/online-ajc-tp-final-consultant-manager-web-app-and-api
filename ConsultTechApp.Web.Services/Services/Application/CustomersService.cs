using System.Net.Http.Json;
using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Dtos.Application.Customers;

namespace ConsultTechApp.Web.Services.Services.Application;

internal class CustomersService : IApiCustomersService
{
    private readonly HttpClient httpClient;

    public CustomersService(IHttpClientFactory httpClientFactory)
    {
        this.httpClient = httpClientFactory.CreateClient("Customers");
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
    {
        var customers = await httpClient.GetFromJsonAsync<IEnumerable<CustomerDto>>("");
        return customers ?? [];
    }

    public async Task<CustomerDetailDto?> GetCustomerAsync(Guid id)
    {
        var customer = await httpClient.GetFromJsonAsync<CustomerDetailDto>($"{id}");
        return customer;
    }

    public async Task CreateCustomerAsync(CreateOrUpdateCustomerDto customerDTO)
    {
        var response = await httpClient.PostAsJsonAsync("", customerDTO);
        _ = response.EnsureSuccessStatusCode();
    }

    public async Task UpdateCustomerAsync(Guid id, CreateOrUpdateCustomerDto customerDto)
    {
        var response = await httpClient.PutAsJsonAsync($"{id}", customerDto);
        _ = response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCustomerAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync($"{id}");
        _ = response.EnsureSuccessStatusCode();
    }
}
