using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Educations;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class EducationRepository(AppDbContext context) : Repository<Education>(context), IEducationRepository
{
    public async Task<Education?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Translations)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Education>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Translations)
            .ToListAsync(cancellationToken);
    }
}
