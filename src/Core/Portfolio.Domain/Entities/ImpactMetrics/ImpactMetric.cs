using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.ImpactMetrics;

public class ImpactMetric : BaseEntity, ITranslatable<ImpactMetricTranslation>
{
    public string Value { get; set; } = string.Empty;   // e.g. "99.9%", "10x"
    public string Color { get; set; } = "amber";         // pink | amber | green
    public int DisplayOrder { get; set; }

    public ICollection<ImpactMetricTranslation> Translations { get; set; } = [];
}
