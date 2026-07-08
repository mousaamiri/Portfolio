using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Stats;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class StatCounterRepository(AppDbContext context) : Repository<StatCounter>(context), IStatCounterRepository
{
    public async Task<StatCounter?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Translations)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<StatCounter>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Translations)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StatCounter>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.IsActive)
            .Include(s => s.Translations)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
