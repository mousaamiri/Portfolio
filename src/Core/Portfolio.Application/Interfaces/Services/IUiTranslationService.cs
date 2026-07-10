using Portfolio.Application.Common;
using Portfolio.Application.DTOs.UiTranslations;

namespace Portfolio.Application.Interfaces.Services;

public interface IUiTranslationService
{
    /// <summary>
    /// Returns the UI-chrome key→value map for the given language. English
    /// (the inline default) yields an empty map — the views/JS use their own
    /// literals in that case.
    /// </summary>
    Task<Result<IReadOnlyDictionary<string, string>>> GetMapAsync(string languageCode, CancellationToken cancellationToken = default);

    // ── Admin CRUD ──
    Task<Result<IReadOnlyList<UiTranslationDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UiTranslationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(UpsertUiTranslationRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpsertUiTranslationRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
