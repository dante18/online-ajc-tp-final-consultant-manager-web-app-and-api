using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConsultTechApp.Web.ViewsModels.Missions;

public class MissionsCreateViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    [Display(Name = "Title")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    [Display(Name = "Start date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    [Display(Name = "End date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Estimate budget is required")]
    [Display(Name = "Estimated Budget")]
    public decimal EstimatedBudget { get; set; }

    [ValidateNever]
    public List<SelectListItem> CustomerList { get; set; }

    [DisplayName("Customer")]
    public Guid CustomerId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate.HasValue && EndDate <= StartDate)
        {
            yield return new ValidationResult(
                "The end date must be later than the start date.",
                new[] { nameof(EndDate) });
        }
    }
}
