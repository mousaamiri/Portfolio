using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Extensions;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class ProjectService(IUnitOfWork unitOfWork) : IProjectService
{
    public async Task<Result<IReadOnlyList<ProjectDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var projects = await unitOfWork.Projects.GetAllWithTranslationsAsync(cancellationToken);

        var dtos = projects.Select(p => MapToDto(p, language)).ToList();
        return Result<IReadOnlyList<ProjectDto>>.Success(dtos);
    }

    public async Task<Result<IReadOnlyList<ProjectDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var projects = await unitOfWork.Projects.GetActiveWithTranslationsAsync(cancellationToken);

        var dtos = projects.Select(p => MapToDto(p, language)).ToList();
        return Result<IReadOnlyList<ProjectDto>>.Success(dtos);
    }

    public async Task<Result<ProjectDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var project = await unitOfWork.Projects.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (project is null)
            return Result<ProjectDto>.Failure($"Project with id '{id}' was not found.");

        var language = ParseLanguage(languageCode);
        return Result<ProjectDto>.Success(MapToDto(project, language));
    }

    public async Task<Result<Guid>> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Slug = request.Slug,
            ThumbnailUrl = request.ThumbnailUrl,
            CoverImageUrl = request.CoverImageUrl,
            PreviewUrl = request.PreviewUrl,
            SourceCodeUrl = request.SourceCodeUrl,
            IsSourcePrivate = request.IsSourcePrivate,
            IsClientProject = request.IsClientProject,
            IsFeatured = request.IsFeatured,
            IsPublished = request.IsPublished,
            StartedAt = request.StartedAt,
            CompletedAt = request.CompletedAt,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in request.Translations)
        {
            project.Translations.Add(new ProjectTranslation
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                Language = ParseLanguage(t.LanguageCode),
                Title = t.Title,
                ShortDescription = t.ShortDescription,
                Description = t.Description,
                Technologies = t.Technologies,
                MetaTitle = t.MetaTitle,
                MetaDescription = t.MetaDescription
            });
        }

        await unitOfWork.Projects.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(project.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = await unitOfWork.Projects.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (project is null)
            return Result<bool>.Failure($"Project with id '{id}' was not found.");

        project.Slug = request.Slug;
        project.ThumbnailUrl = request.ThumbnailUrl;
        project.CoverImageUrl = request.CoverImageUrl;
        project.PreviewUrl = request.PreviewUrl;
        project.SourceCodeUrl = request.SourceCodeUrl;
        project.IsSourcePrivate = request.IsSourcePrivate;
        project.IsClientProject = request.IsClientProject;
        project.IsFeatured = request.IsFeatured;
        project.IsPublished = request.IsPublished;
        project.StartedAt = request.StartedAt;
        project.CompletedAt = request.CompletedAt;
        project.DisplayOrder = request.DisplayOrder;
        project.IsActive = request.IsActive;
        project.UpdatedAt = DateTime.UtcNow;

        project.Translations.Clear();
        foreach (var t in request.Translations)
        {
            project.Translations.Add(new ProjectTranslation
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                Language = ParseLanguage(t.LanguageCode),
                Title = t.Title,
                ShortDescription = t.ShortDescription,
                Description = t.Description,
                Technologies = t.Technologies,
                MetaTitle = t.MetaTitle,
                MetaDescription = t.MetaDescription
            });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await unitOfWork.Projects.GetByIdAsync(id, cancellationToken);
        if (project is null)
            return Result<bool>.Failure($"Project with id '{id}' was not found.");

        unitOfWork.Projects.Delete(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static Language ParseLanguage(string languageCode)
    {
        return Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language)
            ? language
            : Language.En;
    }

    private static ProjectDto MapToDto(Domain.Entities.Projects.Project project, Language language)
    {
        var translation = project.Translations.FirstOrDefault(t => t.Language == language);

        return new ProjectDto
        {
            Id = project.Id,
            Slug = project.Slug,
            ThumbnailUrl = project.ThumbnailUrl,
            CoverImageUrl = project.CoverImageUrl,
            PreviewUrl = project.PreviewUrl,
            SourceCodeUrl = project.SourceCodeUrl,
            IsSourcePrivate = project.IsSourcePrivate,
            IsClientProject = project.IsClientProject,
            IsFeatured = project.IsFeatured,
            IsPublished = project.IsPublished,
            StartedAt = project.StartedAt,
            CompletedAt = project.CompletedAt,
            DisplayOrder = project.DisplayOrder,
            IsActive = project.IsActive,
            CreatedAt = project.CreatedAt,
            Title = translation?.Title ?? string.Empty,
            ShortDescription = translation?.ShortDescription ?? string.Empty,
            Description = translation?.Description,
            Technologies = translation?.Technologies,
            MetaTitle = translation?.MetaTitle,
            MetaDescription = translation?.MetaDescription
        };
    }
}