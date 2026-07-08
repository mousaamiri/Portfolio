using Portfolio.Domain.Entities.Stats;

namespace Portfolio.Application.Interfaces;

public interface IStatCounterRepository : IRepository<StatCounter>
{
    Task<StatCounter?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StatCounter>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StatCounter>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
