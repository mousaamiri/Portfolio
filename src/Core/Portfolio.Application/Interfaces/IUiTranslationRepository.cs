using Portfolio.Domain.Entities.UiTranslations;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Interfaces;

public interface IUiTranslationRepository : IRepository<UiTranslation>
{
    Task<IReadOnlyList<UiTranslation>> GetByLanguageAsync(Language language, CancellationToken cancellationToken = default);

    /// <summary>All rows (every language) ordered by Key then Language — for the admin list.</summary>
    Task<IReadOnlyList<UiTranslation>> GetAllOrderedAsync(CancellationToken cancellationToken = default);

    /// <summary>The row for a specific (Key, Language) pair, or null — for the uniqueness check.</summary>
    Task<UiTranslation?> GetByKeyAsync(string key, Language language, CancellationToken cancellationToken = default);
}
