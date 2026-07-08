using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Interests;

public class InterestTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Label { get; set; } = string.Empty;
}
