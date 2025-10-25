using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Context;
using ConsultTechApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsultTechApp.Core.Repositories;

internal class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationStoreContext _context;

    public CustomerRepository(ApplicationStoreContext context)
    {
        _context = context;
    }

    public List<Customer> GetAllCustomers()
    {
        return _context.Customers.ToList();
    }

    public Customer GetCustomer(Guid id)
    {
        return _context.Customers
            .Include(c => c.Missions)
            .ThenInclude(c => c.Assignments)
            .ThenInclude(a => a.Consultant)
            .Where(c => c.Id == id).FirstOrDefault();
    }

    public void CreateCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();
    }

    public void UpdateCustomer(Customer customer)
    {
        _context.Customers.Update(customer);
        _context.SaveChanges();
    }

    public void DeleteCustomer(Customer customer)
    {
        _context.Customers.Remove(customer);
        _context.SaveChanges();
    }
}