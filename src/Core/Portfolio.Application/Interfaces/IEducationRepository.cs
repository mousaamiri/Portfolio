using Portfolio.Domain.Entities.Educations;

namespace Portfolio.Application.Interfaces;

public interface IEducationRepository : IRepository<Education>
{
    Task<Education?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Education>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Education>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
