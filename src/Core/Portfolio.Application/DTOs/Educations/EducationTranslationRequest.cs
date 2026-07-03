using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Educations;

public class EducationTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string InstitutionName { get; set; } = string.Empty;

    [Required]
    public string Degree { get; set; } = string.Empty;

    [Required]
    public string FieldOfStudy { get; set; } = string.Empty;
    public string? Description { get; set; }
}
