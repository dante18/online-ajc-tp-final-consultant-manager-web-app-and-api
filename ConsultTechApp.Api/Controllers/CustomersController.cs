using ConsultTechApp.Api.Dtos.Customer;
using ConsultTechApp.Api.Dtos.Mission;
using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConsultTechApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> logger;

    private readonly ICustomerRepository customerService;

    public CustomersController(ILogger<CustomersController> logger, ICustomerRepository customerService)
    {
        this.logger = logger;
        this.customerService = customerService;
        this.logger.LogInformation("CustomersController initialized");
    }

    [HttpGet]
    public IResult GetCustomers()
    {
        this.logger.LogInformation("GetCustomers called");

        try
        {
            List<CustomerDto> customers = this.customerService.GetAllCustomers().Select(c => new CustomerDto()
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Industry = c.Industry,
                Address = c.Address,
                ContactName = c.ContactName,
                ContactEmail = c.ContactEmail
            }).ToList();

            this.logger.LogInformation("GetCustomers completed successfully. Found {CustomerCount} customers", customers.Count);

            return Results.Ok(customers);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while fetching customers");

            return Results.InternalServerError();
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> GetCustomerAsync([FromRoute] Guid id)
    {
        this.logger.LogInformation("GetCustomer called with ID: {CustomerId}", id);

        try
        {
            Customer customer = this.customerService.GetCustomer(id);
            if (customer is null)
            {
                this.logger.LogWarning("Customer not found with ID: {CustomerId}", id);
                return Results.NotFound($"Customer not found by id : {id}");
            }

            this.logger.LogInformation("GetCustomer completed successfully for ID: {CustomerId}, Name: {CustomerName}",
                id,
                customer.CompanyName);

            CustomerDetailDto customerDto = new CustomerDetailDto()
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                Industry = customer.Industry,
                Address = customer.Address,
                ContactName = customer.ContactName,
                ContactEmail = customer.ContactEmail,
                Missions = customer.Missions.Select(m => new MissionDto()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    EstimatedBudget = m.EstimatedBudget
                }).ToList()
            };

            return Results.Ok(customerDto);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while fetching customer with ID: {CustomerId}", id);
            return Results.InternalServerError(e);
        }
    }

    [HttpPost()]
    public async Task<IResult> CreateCustomer([FromBody] CreateOrUpdateCustomerDto dto)
    {
        this.logger.LogInformation(
            "CreateCustomer called with CompanyName: {CompanyName}, Address: {Address}, Industry: {Industry}, ContactName: {ContactName}, ContactEmail: {ContactEmail}",
            dto.CompanyName, dto.Address, dto.Industry, dto.ContactName, dto.ContactEmail);

        if (!ModelState.IsValid)
            return Results.BadRequest(ModelState);

        try
        {
            Customer customer = new Customer()
            {
                CompanyName = dto.CompanyName,
                Industry = dto.Industry,
                ContactName = dto.ContactName,
                ContactEmail = dto.ContactEmail,
                Address = dto.Address
            };

            this.logger.LogDebug("Adding new customer to context: {CustomerDetails}",
                new
                {
                    dto.CompanyName,
                    dto.Address,
                    dto.Industry,
                    dto.ContactName,
                    dto.ContactEmail
                }
            );

            this.customerService.CreateCustomer(customer);

            this.logger.LogInformation("CreateCustomer completed successfully. New customer ID: {CustomerId}, Name: {CompanyName}",
                customer.Id,
                customer.CompanyName);

            CustomerDto customerDto = new CustomerDto()
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                Industry = customer.Industry,
                Address = customer.Address,
                ContactName = customer.ContactName,
                ContactEmail = customer.ContactEmail
            };

            return Results.Created($"/api/customers/{customer.Id}", customerDto);
        }
        catch (Exception e)
        {
            this.logger.LogError(e,
                "An error occurred while creating customer with Name: {CompanyName}",
                dto.CompanyName);

            return Results.InternalServerError(e);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdateCustomerAsync([FromRoute] Guid id, [FromBody] CreateOrUpdateCustomerDto dto)
    {
        this.logger.LogInformation("UpdateCustomer called for ID: {CustomerId} with data: {UpdateData}",
            id,
            new
            {
                dto.CompanyName,
                dto.Address,
                dto.Industry,
                dto.ContactName,
                dto.ContactEmail
            });

        if (!ModelState.IsValid)
            return Results.BadRequest(ModelState);

        try
        {
            Customer customer = this.customerService.GetCustomer(id);
            if (customer is null)
            {
                this.logger.LogWarning("Cannot update customer - not found with ID: {CustomerId}", id);
                return Results.NotFound($"Customer not found by id : {id}");
            }

            this.logger.LogDebug("Found existing customer: {CustomerName}",
                customer.CompanyName);

            if (dto.CompanyName != null) customer.CompanyName = dto.CompanyName;
            if (dto.Address != null) customer.Address = dto.Address;
            if (dto.Industry != null) customer.Industry = dto.Industry;
            if (dto.ContactName != null) customer.ContactName = dto.ContactName;
            if (dto.ContactEmail != null) customer.ContactEmail = dto.ContactEmail;

            this.customerService.UpdateCustomer(customer);

            this.logger.LogInformation("UpdateCustomer completed successfully for ID: {CustomerId}", id);

            return Results.NoContent();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while updating customer with ID: {CustomerId}", id);
            return Results.InternalServerError(e);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteCustomer([FromRoute] Guid id)
    {
        this.logger.LogInformation("DeleteCustomer called for ID: {CustomerId}", id);

        try
        {
            Customer customer = this.customerService.GetCustomer(id);
            if (customer is null)
            {
                this.logger.LogWarning("Cannot delete customer - not found with ID: {CustomerId}", id);
                return Results.NotFound($"Customer not found by id : {id}");
            }

            if (customer.Missions.Count > 0)
            {
                this.logger.LogWarning("Cannot delete customer - because missions are assigned to this client with ID: {CustomerId}", id);
                return Results.BadRequest($"Cannot delete customer - because missions are assigned to this client id : {id}");
            }

            this.logger.LogInformation("Deleting customer: ID={CustomerId}, Name='{CustomerName}'",
                customer.Id,
                customer.CompanyName);

            this.customerService.DeleteCustomer(customer);

            this.logger.LogInformation("DeleteCustomer completed successfully for ID: {CustomerId}", id);
            return Results.NoContent();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while deleting customer with ID: {CustomerId}", id);
            return Results.InternalServerError(e);
        }
    }
}
