using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.UiTranslations;
using Portfolio.Domain.Enums;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class UiTranslationRepository(AppDbContext context)
    : Repository<UiTranslation>(context), IUiTranslationRepository
{
    public async Task<IReadOnlyList<UiTranslation>> GetByLanguageAsync(
        Language language, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.Language == language && t.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UiTranslation>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .OrderBy(t => t.Key)
            .ThenBy(t => t.Language)
            .ToListAsync(cancellationToken);
    }

    public async Task<UiTranslation?> GetByKeyAsync(
        string key, Language language, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(t => t.Key == key && t.Language == language, cancellationToken);
    }
}
