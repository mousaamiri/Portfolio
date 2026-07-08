using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Faqs;

public class FaqTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Question { get; set; } = string.Empty;

    [Required]
    public string Answer { get; set; } = string.Empty;
}
