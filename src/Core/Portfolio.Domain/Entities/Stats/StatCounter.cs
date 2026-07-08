using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Stats;

public class StatCounter : BaseEntity, ITranslatable<StatCounterTranslation>
{
    public string Icon { get; set; } = string.Empty;
    public long CountTarget { get; set; }
    public string? Suffix { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<StatCounterTranslation> Translations { get; set; } = [];
}
