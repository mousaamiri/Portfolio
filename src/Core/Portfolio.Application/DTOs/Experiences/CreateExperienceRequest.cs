using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Experiences;

public class CreateExperienceRequest
{
    [Required]
    public string CompanyLogo { get; set; } = string.Empty;

    [Required]
    public string CompanyUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Required]
    [MinLength(1)]
    public List<ExperienceTranslationRequest> Translations { get; set; } = [];
}
