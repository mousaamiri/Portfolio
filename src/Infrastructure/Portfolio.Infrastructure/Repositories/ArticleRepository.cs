using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Articles;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class ArticleRepository(AppDbContext context) : Repository<Article>(context), IArticleRepository
{
    public async Task<Article?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(a => a.Translations)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Article>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(a => a.Translations)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Article>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.IsActive && a.IsPublished)
            .Include(a => a.Translations)
            .OrderByDescending(a => a.PublishDate)
            .ToListAsync(cancellationToken);
    }
}
