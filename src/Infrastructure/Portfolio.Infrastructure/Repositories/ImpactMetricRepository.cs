using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.ImpactMetrics;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class ImpactMetricRepository(AppDbContext context) : Repository<ImpactMetric>(context), IImpactMetricRepository
{
    public async Task<ImpactMetric?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(x => x.Translations).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ImpactMetric>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
        => await DbSet.Include(x => x.Translations).OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ImpactMetric>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
        => await DbSet.Where(x => x.IsActive).Include(x => x.Translations).OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);
}
