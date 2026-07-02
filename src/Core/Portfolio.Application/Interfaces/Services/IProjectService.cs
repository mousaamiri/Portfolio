using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Projects;

namespace Portfolio.Application.Interfaces.Services;

public interface IProjectService
{
    Task<Result<IReadOnlyList<ProjectDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<ProjectDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}