using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Dtos.Application.Skill;
using ConsultTechApp.Web.ViewsModels.Customers;
using ConsultTechApp.Web.ViewsModels.Missions;
using ConsultTechApp.Web.ViewsModels.Skills;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultTechApp.Web.Controllers
{
    public class SkillsController : Controller
    {
        private readonly IApiSkillsService skillsService;
        private readonly IApiCategoriesService categoriesService;
        public SkillsController(IApiSkillsService skillsService, IApiCategoriesService categoriesService)
        {
            this.skillsService = skillsService;
            this.categoriesService = categoriesService;
        }

        // GET: SkillsController
        public async Task<IActionResult> Index()
        {
            var response = await this.skillsService.GetSkillsAsync();
            var model = response.Select(skill => new SkillsViewModel
            {
                Id = skill.Id,
                Name = skill.Name,
                CategoryId = skill.CategoryId,
                CategoryName = skill.CategoryName
            }).ToList();
            return View(model);
        }

        // GET: SkillsController/Details/5
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            try
            {
                var skill = await this.skillsService.GetSkillAsync(id);

                if (skill is null)
                {
                    return this.NotFound();
                }

                SkillsViewModel model = new SkillsViewModel()
                {
                    Id = skill.Id,
                    Name = skill.Name,
                    CategoryName = skill.CategoryName
                };

                return this.View(model);
            }
            catch (Exception e)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "The skill does not exist or has been deleted!";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: SkillsController/Create
        public async Task<IActionResult> Create()
        {
            var categories = await this.categoriesService.GetCategoriesAsync();
            var skillsCreateViewModel = new SkillsCreateViewModel
            {
                CategoriesList = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList()
            };
            return this.View(skillsCreateViewModel);
        }

        // POST: SkillsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SkillsCreateViewModel vm)
        {
            if (!this.ModelState.IsValid)
                return this.View(vm);

            try
            {
                var newSkill = new CreateOrUpdateSkillDto()
                {
                    Name = vm.Name,
                    CategoryId = vm.CategoryId,
                };

                await this.skillsService.CreateSkillAsync(newSkill);

                TempData["messageType"] = "success";
                TempData["message"] = "Skill creation successful!";

                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Skill creation failed!";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: SkillsController/Edit/5
        public async Task<ActionResult> Edit([FromRoute] Guid id)
        {
            try
            {
                var skill = await this.skillsService.GetSkillAsync(id);

                if (skill is null)
                {
                    return this.NotFound();
                }

                var categories = await this.categoriesService.GetCategoriesAsync();

                var skillEditViewModel = new SkillsEditViewModel
                {
                    Id = skill.Id,
                    Name = skill.Name,
                    CategoryId = skill.CategoryId,
                    CategoriesList = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList()

                };

                return this.View(skillEditViewModel);
            }
            catch (Exception e)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "The skill does not exist or has been deleted!";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: SkillsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, SkillsEditViewModel vm)
        {
            if (!this.ModelState.IsValid)
                return this.View(vm);

            try
            {
                var editSkill = new CreateOrUpdateSkillDto()
                {
                    Name = vm.Name,
                    CategoryId = vm.CategoryId
                };

                await this.skillsService.UpdateSkillAsync(id, editSkill);

                TempData["messageType"] = "success";
                TempData["message"] = "Skill update successful!";

                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Customer update failed!";

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: SkillsController/Delete/5
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var skill = await this.skillsService.GetSkillAsync(id);

                if (skill is null)
                {
                    return this.NotFound();
                }

                var skillsViewModel = new SkillsViewModel
                {
                    Id = skill.Id,
                    Name = skill.Name
                };

                return this.View(skillsViewModel);
            }
            catch (Exception e)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "The skill does not exist or has been deleted!";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: SkillsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await this.skillsService.DeleteSkillAsync(id);

                TempData["messageType"] = "success";
                TempData["message"] = "Skill delete successful!";

                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Skill delete failed!";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
