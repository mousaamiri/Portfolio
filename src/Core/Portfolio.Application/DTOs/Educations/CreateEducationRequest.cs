using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Educations;

public class CreateEducationRequest
{
    [Required]
    public string InstitutionLogo { get; set; } = string.Empty;

    [Required]
    public string InstitutionUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public double? Gpa { get; set; }

    [Required]
    [MinLength(1)]
    public List<EducationTranslationRequest> Translations { get; set; } = [];
}
