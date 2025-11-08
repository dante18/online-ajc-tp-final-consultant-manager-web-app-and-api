using ConsultTechApp.Api.Dtos.Category;
using ConsultTechApp.Api.Dtos.Skill;
using ConsultTechApp.Core.Abstractions.Repositories;
using ConsultTechApp.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConsultTechApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Administrator, RH, Manager")]
    public class CategoriesController : Controller
    {
        private readonly ILogger<SkillsController> logger;
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ILogger<SkillsController> logger, ICategoryRepository categoryRepository)
        {
            this.logger = logger;
            this.categoryRepository = categoryRepository;
            this.logger.LogInformation("CategoriesController initialized");
        }


        // ---------------- GET ALL CATEGORIES -------------------

        /// <summary>
        /// Récupère la liste complète des catégories avec leurs compétences.
        /// </summary>
        [HttpGet]
        public IResult GetCategories()
        {
            this.logger.LogInformation("GetCategories called");
            try
            {
                var categories = categoryRepository.GetAllCategories().Select(c => new CategoryDto()
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();
                this.logger.LogInformation("GetCategories completed successfully. Found {CategoryCount} skills", categories.Count);
                return Results.Ok(categories);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while fetching categories");
                return Results.InternalServerError();
            }
        }


        // ------------------- GET BY ID -----------------------

        /// <summary>
        /// Récupère une catégorie spécifique par son ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public IResult GetCategory(Guid id)
        {
            this.logger.LogInformation("GetCategory called with ID: {CategoryId}", id);
            try
            {
                var category = this.categoryRepository.GetCategory(id);
                if (category is null)
                {
                    this.logger.LogWarning("Category not found with ID: {CategoryId}", id);
                    return Results.NotFound($"Category not found by id : {id}");
                }
                this.logger.LogInformation("GetCategory completed successfully for ID: {CategoryId}, Name: {CategoryName}", category.Id, category.Name);
                var categoryDto = new CategoryDetailDto()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Skills = category.Skills.Select(s => new SkillDto()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        CategoryId = category.Id,
                        CategoryName = category.Name
                    }).ToList()
                };
                return Results.Ok(categoryDto);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while fetching category with ID: {CategoryId}", id);
                return Results.InternalServerError();
            }
        }

        // ---------------- CREATE CATEGORY -------------------------

        /// <summary>
        /// Crée une nouvelle catégorie.
        /// </summary>
        [HttpPost]
        //[Authorize(Roles = "Administrator, RH")]
        public async Task<IResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            this.logger.LogInformation("CreateSkill called with Name: {CategoryName}", dto.Name);

            if (!ModelState.IsValid)
                return Results.BadRequest(ModelState);

            try
            {
                var category = new Category()
                {
                    Name = dto.Name,
                };

                this.logger.LogDebug("Adding new category to context: {CategoryDetails}",
                new
                {
                    dto.Name
                }
            );
                this.categoryRepository.CreateCategory(category);
                this.logger.LogInformation("CreateCategory completed successfully for ID: {CategoryId}, Name: {CategoryName}", category.Id, category.Name);
                return Results.Created($"/api/categories/{category.Id}", category);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while creating category with Name: {CategoryName}", dto.Name);
                return Results.InternalServerError();
            }
        }

        // -------------------- UPDATE CATEGORY ------------------------

        /// <summary>
        /// Met à jour une catégorie existante.
        /// </summary>
        [HttpPut("{id:guid}")]
        //[Authorize(Roles = "Administrator, RH")]
        public async Task<IResult> UpdateCategory(Guid id, [FromBody] CreateCategoryDto dto)
        {
            this.logger.LogInformation("UpdateCategory called with ID: {CategoryId}", id);
            if (!ModelState.IsValid)
                return Results.BadRequest(ModelState);
            try
            {
                var existingCategory = this.categoryRepository.GetCategory(id);
                if (existingCategory is null)
                {
                    this.logger.LogWarning("Category not found with ID: {CategoryId}", id);
                    return Results.NotFound($"Category not found by id : {id}");
                }
                existingCategory.Name = dto.Name;
                this.logger.LogDebug("Updating category in context: {CategoryDetails}",
                new
                {
                    id,
                    dto.Name
                }
            );
                this.categoryRepository.UpdateCategory(existingCategory);
                this.logger.LogInformation("UpdateCategy completed successfully for ID: {CategoryId}, Name: {SkillName}", existingCategory.Id, existingCategory.Name);
                return Results.Ok(existingCategory);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while updating category with ID: {CategoryId}", id);
                return Results.InternalServerError();
            }
        }

        // ----------------- DELETE CATEGORY -----------------------

        /// <summary>
        /// Supprime une catégorie.
        /// </summary>
        [HttpDelete("{id:guid}")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IResult> DeleteCategory(Guid id)
        {
            this.logger.LogInformation("DeleteCategory called for ID: {CategoryId}", id);

            try
            {
                Category category = this.categoryRepository.GetCategory(id);
                if (category is null)
                {
                    this.logger.LogWarning("Cannot delete category - not found with ID: {CategoryId}", id);
                    return Results.NotFound($"Skill not found by id : {id}");
                }

                this.logger.LogInformation("Deleting category: ID={CategoryId}, Name='{CategoryName}'",
                    category.Id,
                    category.Name);

                this.categoryRepository.DeleteCategory(category);

                this.logger.LogInformation("DeleteCategory completed successfully for ID: {CategoryId}", id);
                return Results.NoContent();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "An error occurred while deleting category with ID: {CategoryId}", id);
                return Results.InternalServerError(e);
            }
        }
    }
}

