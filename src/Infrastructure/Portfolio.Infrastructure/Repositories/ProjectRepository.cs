using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class ProjectRepository(AppDbContext context) : Repository<Project>(context), IProjectRepository
{
    public async Task<Project?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Translations)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.IsActive)
            .Include(p => p.Translations)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
