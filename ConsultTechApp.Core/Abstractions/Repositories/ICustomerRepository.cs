using ConsultTechApp.Core.Entities;

namespace ConsultTechApp.Core.Abstractions.Repositories;

public interface ICustomerRepository
{
    public List<Customer> GetAllCustomers();

    public Customer GetCustomer(Guid id);

    public void CreateCustomer(Customer customer);

    public void UpdateCustomer(Customer customer);

    public void DeleteCustomer(Customer customer);
}