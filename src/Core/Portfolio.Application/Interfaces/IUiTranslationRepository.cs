using Portfolio.Domain.Entities.UiTranslations;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Interfaces;

public interface IUiTranslationRepository : IRepository<UiTranslation>
{
    Task<IReadOnlyList<UiTranslation>> GetByLanguageAsync(Language language, CancellationToken cancellationToken = default);
}
