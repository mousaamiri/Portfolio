using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Proficiencies;
using Portfolio.Application.Extensions;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Proficiencies;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class ProficiencyGroupService(IUnitOfWork unitOfWork) : IProficiencyGroupService
{
    public async Task<Result<IReadOnlyList<ProficiencyGroupDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var items = await unitOfWork.ProficiencyGroups.GetAllWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<ProficiencyGroupDto>>.Success(items.Select(x => MapToDto(x, language)).ToList());
    }

    public async Task<Result<IReadOnlyList<ProficiencyGroupDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var items = await unitOfWork.ProficiencyGroups.GetActiveWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<ProficiencyGroupDto>>.Success(items.Select(x => MapToDto(x, language)).ToList());
    }

    public async Task<Result<ProficiencyGroupDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.ProficiencyGroups.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (item is null)
            return Result<ProficiencyGroupDto>.Failure($"ProficiencyGroup with id '{id}' was not found.");
        return Result<ProficiencyGroupDto>.Success(MapToDto(item, ParseLanguage(languageCode)));
    }

    public async Task<Result<Guid>> CreateAsync(CreateProficiencyGroupRequest request, CancellationToken cancellationToken = default)
    {
        var item = new ProficiencyGroup
        {
            Id = Guid.NewGuid(),
            Color = request.Color,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        foreach (var t in request.Translations)
            item.Translations.Add(BuildTranslation(item.Id, t));

        await unitOfWork.ProficiencyGroups.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateProficiencyGroupRequest request, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.ProficiencyGroups.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (item is null)
            return Result<bool>.Failure($"ProficiencyGroup with id '{id}' was not found.");

        item.Color = request.Color;
        item.DisplayOrder = request.DisplayOrder;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        item.Translations.SyncTranslations(
            request.Translations.Select(t => BuildTranslation(item.Id, t)).ToList(),
            (existing, incoming) =>
            {
                existing.Title = incoming.Title;
                existing.Items = incoming.Items;
            });

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.ProficiencyGroups.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return Result<bool>.Failure($"ProficiencyGroup with id '{id}' was not found.");

        unitOfWork.ProficiencyGroups.Delete(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static ProficiencyGroupTranslation BuildTranslation(Guid groupId, ProficiencyGroupTranslationRequest t) => new()
    {
        Id = Guid.NewGuid(),
        ProficiencyGroupId = groupId,
        Language = ParseLanguage(t.LanguageCode),
        Title = t.Title,
        Items = t.Items
    };

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;

    private static ProficiencyGroupDto MapToDto(ProficiencyGroup item, Language language)
    {
        var t = item.Translations.FirstOrDefault(x => x.Language == language);
        return new ProficiencyGroupDto
        {
            Id = item.Id,
            Color = item.Color,
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt,
            Title = t?.Title ?? string.Empty,
            Items = t?.Items ?? string.Empty
        };
    }
}
