using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Articles;

namespace Portfolio.Application.Interfaces.Services;

public interface IArticleService
{
    Task<Result<IReadOnlyList<ArticleDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ArticleDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<ArticleDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateArticleRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateArticleRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
