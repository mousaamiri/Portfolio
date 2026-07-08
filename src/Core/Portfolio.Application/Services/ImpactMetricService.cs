using Portfolio.Application.Common;
using Portfolio.Application.DTOs.ImpactMetrics;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.ImpactMetrics;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class ImpactMetricService(IUnitOfWork unitOfWork) : IImpactMetricService
{
    public async Task<Result<IReadOnlyList<ImpactMetricDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var items = await unitOfWork.ImpactMetrics.GetAllWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<ImpactMetricDto>>.Success(items.Select(x => MapToDto(x, language)).ToList());
    }

    public async Task<Result<IReadOnlyList<ImpactMetricDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var items = await unitOfWork.ImpactMetrics.GetActiveWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<ImpactMetricDto>>.Success(items.Select(x => MapToDto(x, language)).ToList());
    }

    public async Task<Result<ImpactMetricDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.ImpactMetrics.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (item is null)
            return Result<ImpactMetricDto>.Failure($"ImpactMetric with id '{id}' was not found.");
        return Result<ImpactMetricDto>.Success(MapToDto(item, ParseLanguage(languageCode)));
    }

    public async Task<Result<Guid>> CreateAsync(CreateImpactMetricRequest request, CancellationToken cancellationToken = default)
    {
        var item = new ImpactMetric
        {
            Id = Guid.NewGuid(),
            Value = request.Value,
            Color = request.Color,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        foreach (var t in request.Translations)
            item.Translations.Add(BuildTranslation(item.Id, t));

        await unitOfWork.ImpactMetrics.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateImpactMetricRequest request, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.ImpactMetrics.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (item is null)
            return Result<bool>.Failure($"ImpactMetric with id '{id}' was not found.");

        item.Value = request.Value;
        item.Color = request.Color;
        item.DisplayOrder = request.DisplayOrder;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        item.Translations.Clear();
        foreach (var t in request.Translations)
            item.Translations.Add(BuildTranslation(item.Id, t));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.ImpactMetrics.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return Result<bool>.Failure($"ImpactMetric with id '{id}' was not found.");

        unitOfWork.ImpactMetrics.Delete(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static ImpactMetricTranslation BuildTranslation(Guid metricId, ImpactMetricTranslationRequest t) => new()
    {
        Id = Guid.NewGuid(),
        ImpactMetricId = metricId,
        Language = ParseLanguage(t.LanguageCode),
        Tag = t.Tag,
        Description = t.Description
    };

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;

    private static ImpactMetricDto MapToDto(ImpactMetric item, Language language)
    {
        var t = item.Translations.FirstOrDefault(x => x.Language == language);
        return new ImpactMetricDto
        {
            Id = item.Id,
            Value = item.Value,
            Color = item.Color,
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt,
            Tag = t?.Tag ?? string.Empty,
            Description = t?.Description ?? string.Empty
        };
    }
}
