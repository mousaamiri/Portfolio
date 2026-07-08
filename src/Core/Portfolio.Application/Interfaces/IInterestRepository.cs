using Portfolio.Domain.Entities.Interests;

namespace Portfolio.Application.Interfaces;

public interface IInterestRepository : IRepository<Interest>
{
    Task<Interest?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Interest>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Interest>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
