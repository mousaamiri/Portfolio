using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Timeline;

public class TimelineEntryTranslation : BaseTranslation
{
    public Guid TimelineEntryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public TimelineEntry TimelineEntry { get; set; } = null!;
}
