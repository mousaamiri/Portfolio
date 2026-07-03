using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Skills;

public class SkillTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
}
