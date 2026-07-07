using Portfolio.Domain.Entities.Experiences;

namespace Portfolio.Application.Interfaces;

public interface IExperienceRepository : IRepository<Experience>
{
    Task<Experience?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Experience>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Experience>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
