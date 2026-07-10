using Portfolio.Application.Common;
using Portfolio.Application.DTOs.UiTranslations;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.UiTranslations;
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

    public async Task<Result<IReadOnlyList<UiTranslationDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await unitOfWork.UiTranslations.GetAllOrderedAsync(cancellationToken);
        return Result<IReadOnlyList<UiTranslationDto>>.Success(rows.Select(MapToDto).ToList());
    }

    public async Task<Result<UiTranslationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await unitOfWork.UiTranslations.GetByIdAsync(id, cancellationToken);
        if (row is null)
            return Result<UiTranslationDto>.Failure($"UI translation with id '{id}' was not found.");

        return Result<UiTranslationDto>.Success(MapToDto(row));
    }

    public async Task<Result<Guid>> CreateAsync(UpsertUiTranslationRequest request, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(request.LanguageCode);
        var key = request.Key.Trim();

        var existing = await unitOfWork.UiTranslations.GetByKeyAsync(key, language, cancellationToken);
        if (existing is not null)
            return Result<Guid>.Failure($"A translation for key '{key}' in '{language}' already exists.");

        var row = new UiTranslation
        {
            Id = Guid.NewGuid(),
            Key = key,
            Language = language,
            Value = request.Value,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.UiTranslations.AddAsync(row, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(row.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpsertUiTranslationRequest request, CancellationToken cancellationToken = default)
    {
        var row = await unitOfWork.UiTranslations.GetByIdAsync(id, cancellationToken);
        if (row is null)
            return Result<bool>.Failure($"UI translation with id '{id}' was not found.");

        var language = ParseLanguage(request.LanguageCode);
        var key = request.Key.Trim();

        // Guard the unique (Key, Language) index if the identity changed.
        if ((row.Key != key || row.Language != language))
        {
            var clash = await unitOfWork.UiTranslations.GetByKeyAsync(key, language, cancellationToken);
            if (clash is not null && clash.Id != id)
                return Result<bool>.Failure($"A translation for key '{key}' in '{language}' already exists.");
        }

        row.Key = key;
        row.Language = language;
        row.Value = request.Value;
        row.IsActive = request.IsActive;
        row.UpdatedAt = DateTime.UtcNow;

        unitOfWork.UiTranslations.Update(row);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await unitOfWork.UiTranslations.GetByIdAsync(id, cancellationToken);
        if (row is null)
            return Result<bool>.Failure($"UI translation with id '{id}' was not found.");

        unitOfWork.UiTranslations.Delete(row);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static UiTranslationDto MapToDto(UiTranslation t) => new()
    {
        Id = t.Id,
        Key = t.Key,
        Language = t.Language.ToString().ToLowerInvariant(),
        Value = t.Value,
        IsActive = t.IsActive,
        CreatedAt = t.CreatedAt
    };

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;
}
