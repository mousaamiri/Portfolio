using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Principles;
using Portfolio.Application.Extensions;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Principles;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class PrincipleService(IUnitOfWork unitOfWork) : IPrincipleService
{
    public async Task<Result<IReadOnlyList<PrincipleDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var items = await unitOfWork.Principles.GetAllWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<PrincipleDto>>.Success(items.Select(x => MapToDto(x, language)).ToList());
    }

    public async Task<Result<IReadOnlyList<PrincipleDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var items = await unitOfWork.Principles.GetActiveWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<PrincipleDto>>.Success(items.Select(x => MapToDto(x, language)).ToList());
    }

    public async Task<Result<PrincipleDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.Principles.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (item is null)
            return Result<PrincipleDto>.Failure($"Principle with id '{id}' was not found.");
        return Result<PrincipleDto>.Success(MapToDto(item, ParseLanguage(languageCode)));
    }

    public async Task<Result<Guid>> CreateAsync(CreatePrincipleRequest request, CancellationToken cancellationToken = default)
    {
        var item = new Principle
        {
            Id = Guid.NewGuid(),
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        foreach (var t in request.Translations)
            item.Translations.Add(BuildTranslation(item.Id, t));

        await unitOfWork.Principles.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdatePrincipleRequest request, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.Principles.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (item is null)
            return Result<bool>.Failure($"Principle with id '{id}' was not found.");

        item.DisplayOrder = request.DisplayOrder;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        item.Translations.SyncTranslations(
            request.Translations.Select(t => BuildTranslation(item.Id, t)).ToList(),
            (existing, incoming) =>
            {
                existing.Title = incoming.Title;
                existing.Description = incoming.Description;
            });

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.Principles.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return Result<bool>.Failure($"Principle with id '{id}' was not found.");

        unitOfWork.Principles.Delete(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static PrincipleTranslation BuildTranslation(Guid principleId, PrincipleTranslationRequest t) => new()
    {
        Id = Guid.NewGuid(),
        PrincipleId = principleId,
        Language = ParseLanguage(t.LanguageCode),
        Title = t.Title,
        Description = t.Description
    };

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;

    private static PrincipleDto MapToDto(Principle item, Language language)
    {
        var t = item.Translations.FirstOrDefault(x => x.Language == language);
        return new PrincipleDto
        {
            Id = item.Id,
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt,
            Title = t?.Title ?? string.Empty,
            Description = t?.Description ?? string.Empty
        };
    }
}
