using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConsultTechApp.Web.ViewsModels.Skills
{
    public class SkillsCreateViewModel
    {
        [Required(ErrorMessage = "Skill name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Skill Name")]
        public string Name { get; set; } = null!;
        [ValidateNever]
        public List<SelectListItem> CategoriesList { get; set; } 

        [DisplayName("Category")]
        public Guid CategoryId { get; set; }
    }
}
