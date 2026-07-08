using Portfolio.Domain.Entities.Principles;

namespace Portfolio.Application.Interfaces;

public interface IPrincipleRepository : IRepository<Principle>
{
    Task<Principle?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Principle>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Principle>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
