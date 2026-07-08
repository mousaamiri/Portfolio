using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.ImpactMetrics;

public class CreateImpactMetricRequest
{
    public string Value { get; set; } = string.Empty;
    public string Color { get; set; } = "amber";
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<ImpactMetricTranslationRequest> Translations { get; set; } = [];
}
