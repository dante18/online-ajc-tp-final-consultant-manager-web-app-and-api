using System.ComponentModel.DataAnnotations;

namespace ConsultTechApp.Api.Dtos.Mission;

public class CreateOrUpdateMissionDto : IValidatableObject
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime? EndDate { get; set; }

    [Required]
    [Range(1, double.MaxValue)]
    public decimal EstimatedBudget { get; set; }

    [Required]
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
