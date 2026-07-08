using Portfolio.Domain.Entities.ImpactMetrics;

namespace Portfolio.Application.Interfaces;

public interface IImpactMetricRepository : IRepository<ImpactMetric>
{
    Task<ImpactMetric?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ImpactMetric>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ImpactMetric>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
