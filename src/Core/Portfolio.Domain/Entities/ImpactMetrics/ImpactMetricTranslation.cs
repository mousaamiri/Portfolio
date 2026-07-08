using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.ImpactMetrics;

public class ImpactMetricTranslation : BaseTranslation
{
    public Guid ImpactMetricId { get; set; }
    public string Tag { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ImpactMetric ImpactMetric { get; set; } = null!;
}
