using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Stats;

public class StatCounterTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Label { get; set; } = string.Empty;
    public string? TipText { get; set; }
    public string? TipAriaLabel { get; set; }
}
