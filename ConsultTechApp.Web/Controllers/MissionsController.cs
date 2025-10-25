using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Dtos.Application.Mission;
using ConsultTechApp.Web.ViewsModels.Consultants;
using ConsultTechApp.Web.ViewsModels.Customers;
using ConsultTechApp.Web.ViewsModels.Missions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultTechApp.Web.Controllers;

public class MissionsController : Controller
{
    private readonly IApiMissionsService missionsService;

    private readonly IApiCustomersService customersService;

    public MissionsController(IApiMissionsService missionsService, IApiCustomersService customersService)
    {
        this.missionsService = missionsService;
        this.customersService = customersService;
    }


    // GET: MissionsController
    public async Task<IActionResult> Index()
    {
        var response = await this.missionsService.GetMissionsAsync();
        MissionsListViewModel model = new MissionsListViewModel()
        {
            Items = response.Select(m => new MissionsViewModel()
            {
                Description = m.Description,
                EndDate = m.EndDate,
                StartDate = m.StartDate,
                EstimatedBudget = m.EstimatedBudget,
                Title = m.Title,
                Id = m.Id,
                Customer = m.Customer.CompanyName,
                Consultants = string.Join(" , ", m.Consultants.Select(c => c.FirstName + " " + c.LastName).ToList())
            }).ToList(),
        };

        return View(model);
    }

    // GET: MissionsController/Details/5
    public async Task<IActionResult> Details([FromRoute] Guid id)
    {
        try
        {
            var mission = await this.missionsService.GetMissionAsync(id);

            if (mission is null)
            {
                return this.NotFound();
            }

            return this.View(new MissionsDetailsViewModel()
            {
                Description = mission.Description,
                EndDate = mission.EndDate,
                StartDate = mission.StartDate,
                EstimatedBudget = mission.EstimatedBudget,
                Title = mission.Title,
                Id = mission.Id,
                Customer = new CustomersViewModel()
                {
                    Address = mission.Customer.Address,
                    CompanyName = mission.Customer.CompanyName,
                    ContactEmail = mission.Customer.ContactEmail,
                    ContactName = mission.Customer.ContactName,
                    Id = mission.Customer.Id,
                    Industry = mission.Customer.Industry
                },
                Consultants = mission.Consultants.Select(c => new ConsultantsViewModel()
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    HireDate = c.HireDate,
                    IsActive = c.IsActive,
                }).ToList()
            });
        }
        catch (Exception e)
        {
            TempData["messageType"] = "error";
            TempData["message"] = "The mission does not exist or has been deleted!";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Create()
    {
        var customers = await this.customersService.GetCustomersAsync();
        var customersCreateViewModel = new MissionsCreateViewModel
        {
            CustomerList = customers.Select(c => new SelectListItem(c.CompanyName, c.Id.ToString())).ToList()
        };

        return this.View(customersCreateViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind("Title, Description, StartDate, EndDate, EstimatedBudget, CustomerId")] MissionsCreateViewModel missionCreateViewModel)
    {
        if (!this.ModelState.IsValid)
            return this.View(missionCreateViewModel);

        try
        {
            var newMission = new CreateOrUpdateMissionDto()
            {
                Title = missionCreateViewModel.Title,
                Description = missionCreateViewModel.Description,
                StartDate = missionCreateViewModel.StartDate,
                EndDate = missionCreateViewModel.EndDate,
                EstimatedBudget = missionCreateViewModel.EstimatedBudget,
                CustomerId = missionCreateViewModel.CustomerId
            };

            await this.missionsService.CreateMissionAsync(newMission);

            TempData["messageType"] = "success";
            TempData["message"] = "Mission creation successful!";

            return this.RedirectToAction(nameof(this.Index));
        }
        catch
        {
            TempData["messageType"] = "error";
            TempData["message"] = "Mission creation failed!";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<ActionResult> Edit([FromRoute] Guid id)
    {
        try
        {
            var mission = await this.missionsService.GetMissionAsync(id);

            if (mission is null)
            {
                return this.NotFound();
            }

            var customers = await this.customersService.GetCustomersAsync();

            var missionEditViewModel = new MissionsEditViewModel
            {
                Id = mission.Id,
                Title = mission.Title,
                CustomerId = mission.Customer.Id,
                CustomerList = customers.Select(c => new SelectListItem(c.CompanyName, c.Id.ToString())).ToList(),
                Description = mission.Description,
                StartDate = mission.StartDate,
                EndDate = mission.EndDate,
                EstimatedBudget = mission.EstimatedBudget
            };

            return this.View(missionEditViewModel);
        }
        catch (Exception e)
        {
            TempData["messageType"] = "error";
            TempData["message"] = "The mission does not exist or has been deleted!";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: MissionsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(Guid id, [Bind("Id, Title, Description, StartDate, EndDate, EstimatedBudget, CustomerId")] MissionsEditViewModel missionEditViewModel)
    {
        if (!this.ModelState.IsValid)
            return this.View(missionEditViewModel);

        try
        {
            var editMission = new CreateOrUpdateMissionDto()
            {
                Title = missionEditViewModel.Title,
                Description = missionEditViewModel.Description,
                StartDate = missionEditViewModel.StartDate,
                EndDate = missionEditViewModel.EndDate,
                EstimatedBudget = missionEditViewModel.EstimatedBudget,
                CustomerId = missionEditViewModel.CustomerId
            };

            await this.missionsService.UpdateMissionAsync(id, editMission);

            TempData["messageType"] = "success";
            TempData["message"] = "Mission update successful!";

            return this.RedirectToAction(nameof(this.Index));
        }
        catch
        {
            TempData["messageType"] = "error";
            TempData["message"] = "Mission update failed!";

            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            var mission = await this.missionsService.GetMissionAsync(id);

            if (mission is null)
            {
                return this.NotFound();
            }

            var missionDeleteViewModel = new MissionsDeleteViewModel
            {
                Title = mission.Title,
                Description = mission.Description,
                StartDate = mission.StartDate,
                EndDate = mission.EndDate,
                EstimatedBudget = mission.EstimatedBudget,
                Customer = mission.Customer.CompanyName,
                Id = mission.Id
            };

            return this.View(missionDeleteViewModel);
        }
        catch (Exception e)
        {
            TempData["messageType"] = "error";
            TempData["message"] = "The mission does not exist or has been deleted!";
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
            await this.missionsService.DeleteMissionAsync(id);

            TempData["messageType"] = "success";
            TempData["message"] = "Mission delete successful!";

            return this.RedirectToAction(nameof(this.Index));
        }
        catch
        {
            TempData["messageType"] = "error";
            TempData["message"] = "Mission delete failed!";

            return RedirectToAction(nameof(Index));
        }
    }
}