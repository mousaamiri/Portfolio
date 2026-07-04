using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Educations;
using Portfolio.Application.Extensions;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Educations;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class EducationService(IUnitOfWork unitOfWork) : IEducationService
{
    public async Task<Result<IReadOnlyList<EducationDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var educations = await unitOfWork.Educations.GetAllAsync(cancellationToken);

        var dtos = educations.Select(e => MapToDto(e, language)).ToList();
        return Result<IReadOnlyList<EducationDto>>.Success(dtos);
    }

    public async Task<Result<EducationDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var education = await unitOfWork.Educations.GetByIdAsync(id, cancellationToken);
        if (education is null)
            return Result<EducationDto>.Failure($"Education with id '{id}' was not found.");

        var language = ParseLanguage(languageCode);
        return Result<EducationDto>.Success(MapToDto(education, language));
    }

    public async Task<Result<Guid>> CreateAsync(CreateEducationRequest request, CancellationToken cancellationToken = default)
    {
        var education = new Education
        {
            Id = Guid.NewGuid(),
            InstitutionLogo = request.InstitutionLogo,
            InstitutionUrl = request.InstitutionUrl,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Gpa = request.Gpa,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in request.Translations)
        {
            education.Translations.Add(new EducationTranslation
            {
                Id = Guid.NewGuid(),
                EducationId = education.Id,
                Language = ParseLanguage(t.LanguageCode),
                InstitutionName = t.InstitutionName,
                Degree = t.Degree,
                FieldOfStudy = t.FieldOfStudy,
                Description = t.Description
            });
        }

        await unitOfWork.Educations.AddAsync(education, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(education.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateEducationRequest request, CancellationToken cancellationToken = default)
    {
        var education = await unitOfWork.Educations.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (education is null)
            return Result<bool>.Failure($"Education with id '{id}' was not found.");

        education.InstitutionLogo = request.InstitutionLogo;
        education.InstitutionUrl = request.InstitutionUrl;
        education.StartDate = request.StartDate;
        education.EndDate = request.EndDate;
        education.Gpa = request.Gpa;
        education.IsActive = request.IsActive;
        education.UpdatedAt = DateTime.UtcNow;

        education.Translations.SyncTranslations(
            request.Translations.Select(t => new EducationTranslation
            {
                Language = ParseLanguage(t.LanguageCode),
                InstitutionName = t.InstitutionName,
                Degree = t.Degree,
                FieldOfStudy = t.FieldOfStudy,
                Description = t.Description
            }).ToList(),
            (existing, incoming) =>
            {
                existing.InstitutionName = incoming.InstitutionName;
                existing.Degree = incoming.Degree;
                existing.FieldOfStudy = incoming.FieldOfStudy;
                existing.Description = incoming.Description;
            });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var education = await unitOfWork.Educations.GetByIdAsync(id, cancellationToken);
        if (education is null)
            return Result<bool>.Failure($"Education with id '{id}' was not found.");

        unitOfWork.Educations.Delete(education);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static Language ParseLanguage(string languageCode)
    {
        return Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language)
            ? language
            : Language.En;
    }

    private static EducationDto MapToDto(Education education, Language language)
    {
        var translation = education.Translations.FirstOrDefault(t => t.Language == language);

        return new EducationDto
        {
            Id = education.Id,
            InstitutionLogo = education.InstitutionLogo,
            InstitutionUrl = education.InstitutionUrl,
            StartDate = education.StartDate,
            EndDate = education.EndDate,
            Gpa = education.Gpa,
            IsActive = education.IsActive,
            CreatedAt = education.CreatedAt,
            InstitutionName = translation?.InstitutionName ?? string.Empty,
            Degree = translation?.Degree ?? string.Empty,
            FieldOfStudy = translation?.FieldOfStudy ?? string.Empty,
            Description = translation?.Description
        };
    }
}