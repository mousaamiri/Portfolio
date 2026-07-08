using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Timeline;

public class TimelineEntry : BaseEntity, ITranslatable<TimelineEntryTranslation>
{
    public string Year { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ICollection<TimelineEntryTranslation> Translations { get; set; } = [];
}
