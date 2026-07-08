using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Profiles;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class ProfileRepository(AppDbContext context) : Repository<Profile>(context), IProfileRepository
{
    public async Task<Profile?> GetFirstActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.IsActive)
            .Include(p => p.Translations)
            .OrderBy(p => p.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Profile?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
