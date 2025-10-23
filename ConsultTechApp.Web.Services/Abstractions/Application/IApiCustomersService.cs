using ConsultTechApp.Web.Services.Dtos.Application.Customers;

namespace ConsultTechApp.Web.Services.Abstractions.Application;

public interface IApiCustomersService
{
    public Task<IEnumerable<CustomerDto>> GetCustomersAsync();

    public Task<CustomerDetailDto?> GetCustomerAsync(Guid id);

    public Task CreateCustomerAsync(CreateOrUpdateCustomerDto customerDto);

    public Task UpdateCustomerAsync(Guid id, CreateOrUpdateCustomerDto customerDto);

    public Task DeleteCustomerAsync(Guid id);
}