using Portfolio.Domain.Entities.Articles;

namespace Portfolio.Application.Interfaces;

public interface IArticleRepository : IRepository<Article>
{
    Task<Article?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Article>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Article>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
