using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Interests;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class InterestRepository(AppDbContext context) : Repository<Interest>(context), IInterestRepository
{
    public async Task<Interest?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.Translations)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Interest>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.Translations)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Interest>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(i => i.IsActive)
            .Include(i => i.Translations)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
