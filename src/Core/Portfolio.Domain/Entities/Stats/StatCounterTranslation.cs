using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Stats;

public class StatCounterTranslation : BaseTranslation
{
    public Guid StatCounterId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? TipText { get; set; }
    public string? TipAriaLabel { get; set; }

    public StatCounter StatCounter { get; set; } = null!;
}
