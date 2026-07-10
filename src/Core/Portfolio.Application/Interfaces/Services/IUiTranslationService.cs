using Portfolio.Application.Common;

namespace Portfolio.Application.Interfaces.Services;

public interface IUiTranslationService
{
    /// <summary>
    /// Returns the UI-chrome key→value map for the given language. English
    /// (the inline default) yields an empty map — the views/JS use their own
    /// literals in that case.
    /// </summary>
    Task<Result<IReadOnlyDictionary<string, string>>> GetMapAsync(string languageCode, CancellationToken cancellationToken = default);
}
