using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Skills;
using Portfolio.Application.Extensions;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Skills;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class SkillService(IUnitOfWork unitOfWork) : ISkillService
{
    public async Task<Result<IReadOnlyList<SkillDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var skills = await unitOfWork.Skills.GetAllAsync(cancellationToken);

        var dtos = skills.Select(s => MapToDto(s, language)).ToList();
        return Result<IReadOnlyList<SkillDto>>.Success(dtos);
    }

    public async Task<Result<SkillDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var skill = await unitOfWork.Skills.GetByIdAsync(id, cancellationToken);
        if (skill is null)
            return Result<SkillDto>.Failure($"Skill with id '{id}' was not found.");

        var language = ParseLanguage(languageCode);
        return Result<SkillDto>.Success(MapToDto(skill, language));
    }

    public async Task<Result<Guid>> CreateAsync(CreateSkillRequest request, CancellationToken cancellationToken = default)
    {
        var skill = new Skill
        {
            Id = Guid.NewGuid(),
            IconUrl = request.IconUrl,
            Proficiency = request.Proficiency,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in request.Translations)
        {
            skill.Translations.Add(new SkillTranslation
            {
                Id = Guid.NewGuid(),
                SkillId = skill.Id,
                Language = ParseLanguage(t.LanguageCode),
                Name = t.Name,
                Description = t.Description,
                Category = t.Category
            });
        }

        await unitOfWork.Skills.AddAsync(skill, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(skill.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateSkillRequest request, CancellationToken cancellationToken = default)
    {
        var skill = await unitOfWork.Skills.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (skill is null)
            return Result<bool>.Failure($"Skill with id '{id}' was not found.");

        skill.IconUrl = request.IconUrl;
        skill.Proficiency = request.Proficiency;
        skill.DisplayOrder = request.DisplayOrder;
        skill.IsActive = request.IsActive;
        skill.UpdatedAt = DateTime.UtcNow;

        skill.Translations.Clear();
        foreach (var t in request.Translations)
        {
            skill.Translations.Add(new SkillTranslation
            {
                Id = Guid.NewGuid(),
                SkillId = skill.Id,
                Language = ParseLanguage(t.LanguageCode),
                Name = t.Name,
                Description = t.Description,
                Category = t.Category
            });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var skill = await unitOfWork.Skills.GetByIdAsync(id, cancellationToken);
        if (skill is null)
            return Result<bool>.Failure($"Skill with id '{id}' was not found.");

        unitOfWork.Skills.Delete(skill);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static Language ParseLanguage(string languageCode)
    {
        return Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language)
            ? language
            : Language.En;
    }

    private static SkillDto MapToDto(Skill skill, Language language)
    {
        var translation = skill.Translations.FirstOrDefault(t => t.Language == language);

        return new SkillDto
        {
            Id = skill.Id,
            IconUrl = skill.IconUrl,
            Proficiency = skill.Proficiency,
            DisplayOrder = skill.DisplayOrder,
            IsActive = skill.IsActive,
            CreatedAt = skill.CreatedAt,
            Name = translation?.Name ?? string.Empty,
            Description = translation?.Description,
            Category = translation?.Category
        };
    }
}