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
}
