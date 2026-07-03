using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Experiences;

public class ExperienceTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public string JobTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
}
