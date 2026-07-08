using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Proficiencies;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class ProficiencyGroupRepository(AppDbContext context) : Repository<ProficiencyGroup>(context), IProficiencyGroupRepository
{
    public async Task<ProficiencyGroup?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(x => x.Translations).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProficiencyGroup>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
        => await DbSet.Include(x => x.Translations).OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ProficiencyGroup>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
        => await DbSet.Where(x => x.IsActive).Include(x => x.Translations).OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);
}
