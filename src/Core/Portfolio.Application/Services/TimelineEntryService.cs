using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Timeline;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Timeline;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class TimelineEntryService(IUnitOfWork unitOfWork) : ITimelineEntryService
{
    public async Task<Result<IReadOnlyList<TimelineEntryDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var entries = await unitOfWork.TimelineEntries.GetAllWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<TimelineEntryDto>>.Success(entries.Select(e => MapToDto(e, language)).ToList());
    }

    public async Task<Result<IReadOnlyList<TimelineEntryDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var entries = await unitOfWork.TimelineEntries.GetActiveWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<TimelineEntryDto>>.Success(entries.Select(e => MapToDto(e, language)).ToList());
    }

    public async Task<Result<TimelineEntryDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var entry = await unitOfWork.TimelineEntries.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (entry is null)
            return Result<TimelineEntryDto>.Failure($"TimelineEntry with id '{id}' was not found.");

        return Result<TimelineEntryDto>.Success(MapToDto(entry, ParseLanguage(languageCode)));
    }

    public async Task<Result<Guid>> CreateAsync(CreateTimelineEntryRequest request, CancellationToken cancellationToken = default)
    {
        var entry = new TimelineEntry
        {
            Id = Guid.NewGuid(),
            Year = request.Year,
            Icon = request.Icon,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in request.Translations)
            entry.Translations.Add(BuildTranslation(entry.Id, t));

        await unitOfWork.TimelineEntries.AddAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entry.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateTimelineEntryRequest request, CancellationToken cancellationToken = default)
    {
        var entry = await unitOfWork.TimelineEntries.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (entry is null)
            return Result<bool>.Failure($"TimelineEntry with id '{id}' was not found.");

        entry.Year = request.Year;
        entry.Icon = request.Icon;
        entry.DisplayOrder = request.DisplayOrder;
        entry.IsActive = request.IsActive;
        entry.UpdatedAt = DateTime.UtcNow;

        entry.Translations.Clear();
        foreach (var t in request.Translations)
            entry.Translations.Add(BuildTranslation(entry.Id, t));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entry = await unitOfWork.TimelineEntries.GetByIdAsync(id, cancellationToken);
        if (entry is null)
            return Result<bool>.Failure($"TimelineEntry with id '{id}' was not found.");

        unitOfWork.TimelineEntries.Delete(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static TimelineEntryTranslation BuildTranslation(Guid entryId, TimelineEntryTranslationRequest t) => new()
    {
        Id = Guid.NewGuid(),
        TimelineEntryId = entryId,
        Language = ParseLanguage(t.LanguageCode),
        Title = t.Title,
        Description = t.Description
    };

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;

    private static TimelineEntryDto MapToDto(TimelineEntry entry, Language language)
    {
        var t = entry.Translations.FirstOrDefault(x => x.Language == language);
        return new TimelineEntryDto
        {
            Id = entry.Id,
            Year = entry.Year,
            Icon = entry.Icon,
            DisplayOrder = entry.DisplayOrder,
            IsActive = entry.IsActive,
            CreatedAt = entry.CreatedAt,
            Title = t?.Title ?? string.Empty,
            Description = t?.Description
        };
    }
}
