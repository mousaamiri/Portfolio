using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Experiences;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class ExperienceRepository(AppDbContext context) : Repository<Experience>(context), IExperienceRepository
{
    public async Task<Experience?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Translations)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
