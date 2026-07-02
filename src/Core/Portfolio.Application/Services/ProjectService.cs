using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class ProjectService(IUnitOfWork unitOfWork) : IProjectService
{
    public async Task<Result<IReadOnlyList<ProjectDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var projects = await unitOfWork.Projects.GetAllAsync(cancellationToken);

        var dtos = projects.Select(p => MapToDto(p, language)).ToList();
        return Result<IReadOnlyList<ProjectDto>>.Success(dtos);
    }

    public async Task<Result<ProjectDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var project = await unitOfWork.Projects.GetByIdAsync(id, cancellationToken);
        if (project is null)
            return Result<ProjectDto>.Failure($"Project with id '{id}' was not found.");

        var language = ParseLanguage(languageCode);
        return Result<ProjectDto>.Success(MapToDto(project, language));
    }

    public Task<Result<Guid>> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
            ImageUrl = project.ImageUrl,
            DemoUrl = project.DemoUrl,
            SourceCodeUrl = project.SourceCodeUrl,
            DisplayOrder = project.DisplayOrder,
            IsActive = project.IsActive,
            CreatedAt = project.CreatedAt,
            Title = translation?.Title ?? string.Empty,
            Description = translation?.Description,
            Technologies = translation?.Technologies
        };
    }
}