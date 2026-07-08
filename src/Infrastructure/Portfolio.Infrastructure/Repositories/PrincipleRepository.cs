using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Principles;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class PrincipleRepository(AppDbContext context) : Repository<Principle>(context), IPrincipleRepository
{
    public async Task<Principle?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(x => x.Translations).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Principle>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
        => await DbSet.Include(x => x.Translations).OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Principle>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
        => await DbSet.Where(x => x.IsActive).Include(x => x.Translations).OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);
}
