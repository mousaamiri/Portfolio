using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.ImpactMetrics;

public class ImpactMetricTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Tag { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
