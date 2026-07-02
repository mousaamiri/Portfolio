using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Experiences;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Experiences;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class ExperienceService(IUnitOfWork unitOfWork) : IExperienceService
{
    public async Task<Result<IReadOnlyList<ExperienceDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var experiences = await unitOfWork.Experiences.GetAllAsync(cancellationToken);

        var dtos = experiences.Select(e => MapToDto(e, language)).ToList();
        return Result<IReadOnlyList<ExperienceDto>>.Success(dtos);
    }

    public async Task<Result<ExperienceDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var experience = await unitOfWork.Experiences.GetByIdAsync(id, cancellationToken);
        if (experience is null)
            return Result<ExperienceDto>.Failure($"Experience with id '{id}' was not found.");

        var language = ParseLanguage(languageCode);
        return Result<ExperienceDto>.Success(MapToDto(experience, language));
    }

    public async Task<Result<Guid>> CreateAsync(CreateExperienceRequest request, CancellationToken cancellationToken = default)
    {
        var experience = new Experience
        {
            Id = Guid.NewGuid(),
            CompanyLogo = request.CompanyLogo,
            CompanyUrl = request.CompanyUrl,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in request.Translations)
        {
            experience.Translations.Add(new ExperienceTranslation
            {
                Id = Guid.NewGuid(),
                ExperienceId = experience.Id,
                Language = ParseLanguage(t.LanguageCode),
                CompanyName = t.CompanyName,
                JobTitle = t.JobTitle,
                Description = t.Description,
                Location = t.Location
            });
        }

        await unitOfWork.Experiences.AddAsync(experience, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(experience.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateExperienceRequest request, CancellationToken cancellationToken = default)
    {
        var experience = await unitOfWork.Experiences.GetByIdAsync(id, cancellationToken);
        if (experience is null)
            return Result<bool>.Failure($"Experience with id '{id}' was not found.");

        experience.CompanyLogo = request.CompanyLogo;
        experience.CompanyUrl = request.CompanyUrl;
        experience.StartDate = request.StartDate;
        experience.EndDate = request.EndDate;
        experience.IsActive = request.IsActive;
        experience.UpdatedAt = DateTime.UtcNow;

        experience.Translations.Clear();
        foreach (var t in request.Translations)
        {
            experience.Translations.Add(new ExperienceTranslation
            {
                Id = Guid.NewGuid(),
                ExperienceId = experience.Id,
                Language = ParseLanguage(t.LanguageCode),
                CompanyName = t.CompanyName,
                JobTitle = t.JobTitle,
                Description = t.Description,
                Location = t.Location
            });
        }

        unitOfWork.Experiences.Update(experience);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var experience = await unitOfWork.Experiences.GetByIdAsync(id, cancellationToken);
        if (experience is null)
            return Result<bool>.Failure($"Experience with id '{id}' was not found.");

        unitOfWork.Experiences.Delete(experience);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static Language ParseLanguage(string languageCode)
    {
        return Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language)
            ? language
            : Language.En;
    }

    private static ExperienceDto MapToDto(Experience experience, Language language)
    {
        var translation = experience.Translations.FirstOrDefault(t => t.Language == language);

        return new ExperienceDto
        {
            Id = experience.Id,
            CompanyLogo = experience.CompanyLogo,
            CompanyUrl = experience.CompanyUrl,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            IsActive = experience.IsActive,
            CreatedAt = experience.CreatedAt,
            CompanyName = translation?.CompanyName ?? string.Empty,
            JobTitle = translation?.JobTitle ?? string.Empty,
            Description = translation?.Description,
            Location = translation?.Location
        };
    }
}