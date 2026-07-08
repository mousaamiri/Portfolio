using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Stats;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Stats;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class StatCounterService(IUnitOfWork unitOfWork) : IStatCounterService
{
    public async Task<Result<IReadOnlyList<StatCounterDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var stats = await unitOfWork.StatCounters.GetAllWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<StatCounterDto>>.Success(stats.Select(s => MapToDto(s, language)).ToList());
    }

    public async Task<Result<IReadOnlyList<StatCounterDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var stats = await unitOfWork.StatCounters.GetActiveWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<StatCounterDto>>.Success(stats.Select(s => MapToDto(s, language)).ToList());
    }

    public async Task<Result<StatCounterDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var stat = await unitOfWork.StatCounters.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (stat is null)
            return Result<StatCounterDto>.Failure($"StatCounter with id '{id}' was not found.");

        return Result<StatCounterDto>.Success(MapToDto(stat, ParseLanguage(languageCode)));
    }

    public async Task<Result<Guid>> CreateAsync(CreateStatCounterRequest request, CancellationToken cancellationToken = default)
    {
        var stat = new StatCounter
        {
            Id = Guid.NewGuid(),
            Icon = request.Icon,
            CountTarget = request.CountTarget,
            Suffix = request.Suffix,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in request.Translations)
            stat.Translations.Add(BuildTranslation(stat.Id, t));

        await unitOfWork.StatCounters.AddAsync(stat, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(stat.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateStatCounterRequest request, CancellationToken cancellationToken = default)
    {
        var stat = await unitOfWork.StatCounters.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (stat is null)
            return Result<bool>.Failure($"StatCounter with id '{id}' was not found.");

        stat.Icon = request.Icon;
        stat.CountTarget = request.CountTarget;
        stat.Suffix = request.Suffix;
        stat.DisplayOrder = request.DisplayOrder;
        stat.IsActive = request.IsActive;
        stat.UpdatedAt = DateTime.UtcNow;

        stat.Translations.Clear();
        foreach (var t in request.Translations)
            stat.Translations.Add(BuildTranslation(stat.Id, t));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var stat = await unitOfWork.StatCounters.GetByIdAsync(id, cancellationToken);
        if (stat is null)
            return Result<bool>.Failure($"StatCounter with id '{id}' was not found.");

        unitOfWork.StatCounters.Delete(stat);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static StatCounterTranslation BuildTranslation(Guid statId, StatCounterTranslationRequest t) => new()
    {
        Id = Guid.NewGuid(),
        StatCounterId = statId,
        Language = ParseLanguage(t.LanguageCode),
        Label = t.Label,
        TipText = t.TipText,
        TipAriaLabel = t.TipAriaLabel
    };

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;

    private static StatCounterDto MapToDto(StatCounter stat, Language language)
    {
        var t = stat.Translations.FirstOrDefault(x => x.Language == language);
        return new StatCounterDto
        {
            Id = stat.Id,
            Icon = stat.Icon,
            CountTarget = stat.CountTarget,
            Suffix = stat.Suffix,
            DisplayOrder = stat.DisplayOrder,
            IsActive = stat.IsActive,
            CreatedAt = stat.CreatedAt,
            Label = t?.Label ?? string.Empty,
            TipText = t?.TipText,
            TipAriaLabel = t?.TipAriaLabel
        };
    }
}
