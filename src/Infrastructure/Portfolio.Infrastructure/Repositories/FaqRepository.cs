using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Faqs;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class FaqRepository(AppDbContext context) : Repository<Faq>(context), IFaqRepository
{
    public async Task<Faq?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(f => f.Translations)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Faq>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(f => f.Translations)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Faq>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(f => f.IsActive)
            .Include(f => f.Translations)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
