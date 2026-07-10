using Portfolio.Application.Common;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class UiTranslationService(IUnitOfWork unitOfWork) : IUiTranslationService
{
    public async Task<Result<IReadOnlyDictionary<string, string>>> GetMapAsync(
        string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);

        // English is the inline default in the views/JS — no DB round-trip needed.
        if (language == Language.En)
            return Result<IReadOnlyDictionary<string, string>>.Success(
                new Dictionary<string, string>());

        var rows = await unitOfWork.UiTranslations.GetByLanguageAsync(language, cancellationToken);

        // Last-write-wins on the off chance of a duplicate key (the unique index
        // prevents it, but the dictionary build must not throw regardless).
        var map = new Dictionary<string, string>(rows.Count);
        foreach (var r in rows)
            map[r.Key] = r.Value;

        return Result<IReadOnlyDictionary<string, string>>.Success(map);
    }

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;
}
