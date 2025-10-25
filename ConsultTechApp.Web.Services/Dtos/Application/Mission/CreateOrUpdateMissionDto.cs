using System.ComponentModel.DataAnnotations;

namespace ConsultTechApp.Web.Services.Dtos.Application.Mission;

public class CreateOrUpdateMissionDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public decimal EstimatedBudget { get; set; }

    [Required]
    public Guid? CustomerId { get; set; }
}
