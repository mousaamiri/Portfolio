using Portfolio.Domain.Entities.Timeline;

namespace Portfolio.Application.Interfaces;

public interface ITimelineEntryRepository : IRepository<TimelineEntry>
{
    Task<TimelineEntry?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimelineEntry>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimelineEntry>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
