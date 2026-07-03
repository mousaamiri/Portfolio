using Portfolio.Domain.Entities.Projects;

namespace Portfolio.Application.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
}
