using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Dtos.Application.Customers;
using ConsultTechApp.Web.ViewsModels.Customers;
using ConsultTechApp.Web.ViewsModels.Missions;
using Microsoft.AspNetCore.Mvc;

namespace ConsultTechApp.Web.Controllers;

public class CustomersController : Controller
{
    private readonly IApiCustomersService customersService;

    public CustomersController(IApiCustomersService customersService)
    {
        this.customersService = customersService;
    }


    // GET: CustomersController
    public async Task<IActionResult> Index()
    {
        var response = await this.customersService.GetCustomersAsync();
        CustomersListViewModel model = new CustomersListViewModel()
        {
            Items = response.Select(customer => new CustomersViewModel()
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                Industry = customer.Industry,
                Address = customer.Address,
                ContactName = customer.ContactName,
                ContactEmail = customer.ContactEmail
            }).ToList(),
        };

        return View(model);
    }

    // GET: CustomersController/Details/5
    public async Task<IActionResult> Details([FromRoute] Guid id)
    {
        try
        {
            var customer = await this.customersService.GetCustomerAsync(id);

            if (customer is null)
            {
                return this.NotFound();
            }

            List<MissionsViewModel> missions = new List<MissionsViewModel>();
            if (customer.Missions is not null)
            {
                missions = customer.Missions.Select(m => new MissionsViewModel()
                {
                    Description = m.Description,
                    EndDate = m.EndDate,
                    StartDate = m.StartDate,
                    EstimatedBudget = m.EstimatedBudget,
                    Title = m.Title,
                    Id = m.Id,
                    Customer = customer.CompanyName,
                    Consultants = m.Consultants is null ? "Not assigned" : string.Join(" , ", m.Consultants.Select(c => c.FirstName + " " + c.LastName).ToList())
                }).ToList();
            }

            CustomersDetailsViewModel model = new CustomersDetailsViewModel()
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                Industry = customer.Industry,
                Address = customer.Address,
                ContactName = customer.ContactName,
                ContactEmail = customer.ContactEmail,
                Missions = missions
            };

            return this.View(model);
        }
        catch (Exception e)
        {
            TempData["messageType"] = "error";
            TempData["message"] = "The customer does not exist or has been deleted!";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create()
    {
        return this.View(new CustomersCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind("CompanyName,Industry,Address,ContactName,ContactEmail")] CustomersCreateViewModel customerCreateViewModel)
    {
        if (!this.ModelState.IsValid)
            return this.View(customerCreateViewModel);

        try
        {
            var newCustomer = new CreateOrUpdateCustomerDto()
            {
                CompanyName = customerCreateViewModel.CompanyName,
                Industry = customerCreateViewModel.Industry,
                Address = customerCreateViewModel.Address,
                ContactEmail = customerCreateViewModel.ContactEmail,
                ContactName = customerCreateViewModel.ContactName
            };

            await this.customersService.CreateCustomerAsync(newCustomer);

            TempData["messageType"] = "success";
            TempData["message"] = "Customer creation successful!";

            return this.RedirectToAction(nameof(this.Index));
        }
        catch
        {
            TempData["messageType"] = "error";
            TempData["message"] = "Customer creation failed!";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<ActionResult> Edit([FromRoute] Guid id)
    {
        try
        {
            var customer = await this.customersService.GetCustomerAsync(id);

            if (customer is null)
            {
                return this.NotFound();
            }

            var customerEditViewModel = new CustomersEditViewModel
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                Industry = customer.Industry,
                Address = customer.Address,
                ContactEmail = customer.ContactEmail,
                ContactName = customer.ContactName
            };

            return this.View(customerEditViewModel);
        }
        catch (Exception e)
        {
            TempData["messageType"] = "error";
            TempData["message"] = "The customer does not exist or has been deleted!";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: CustomersController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(Guid id, [Bind("Id,CompanyName,Industry,Address,ContactName,ContactEmail")] CustomersEditViewModel customerEditViewModel)
    {
        if (!this.ModelState.IsValid)
            return this.View(customerEditViewModel);

        try
        {
            var editCustomer = new CreateOrUpdateCustomerDto()
            {
                CompanyName = customerEditViewModel.CompanyName,
                Industry = customerEditViewModel.Industry,
                Address = customerEditViewModel.Address,
                ContactEmail = customerEditViewModel.ContactEmail,
                ContactName = customerEditViewModel.ContactName
            };

            await this.customersService.UpdateCustomerAsync(id, editCustomer);

            TempData["messageType"] = "success";
            TempData["message"] = "Customer update successful!";

            return this.RedirectToAction(nameof(this.Index));
        }
        catch
        {
            TempData["messageType"] = "error";
            TempData["message"] = "Customer update failed!";

            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            var customer = await this.customersService.GetCustomerAsync(id);

            if (customer is null)
            {
                return this.NotFound();
            }

            var customerDeleteViewModel = new CustomersDeleteViewModel
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                Industry = customer.Industry,
                Address = customer.Address,
                ContactEmail = customer.ContactEmail,
                ContactName = customer.ContactName
            };

            return this.View(customerDeleteViewModel);
        }
        catch (Exception e)
        {
            TempData["messageType"] = "error";
            TempData["message"] = "The customer does not exist or has been deleted!";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: DrinksController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await this.customersService.DeleteCustomerAsync(id);

            TempData["messageType"] = "success";
            TempData["message"] = "Customer delete successful!";

            return this.RedirectToAction(nameof(this.Index));
        }
        catch
        {
            TempData["messageType"] = "error";
            TempData["message"] = "Customer delete failed!";

            return RedirectToAction(nameof(Index));
        }
    }
}