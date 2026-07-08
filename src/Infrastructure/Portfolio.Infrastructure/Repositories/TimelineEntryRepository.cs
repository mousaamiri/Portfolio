using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Timeline;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class TimelineEntryRepository(AppDbContext context)
    : Repository<TimelineEntry>(context), ITimelineEntryRepository
{
    public async Task<TimelineEntry?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Translations)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TimelineEntry>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Translations)
            .OrderBy(t => t.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimelineEntry>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.IsActive)
            .Include(t => t.Translations)
            .OrderBy(t => t.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
