using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Proficiencies;

public class ProficiencyGroupTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;
}
