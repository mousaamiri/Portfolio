using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Articles;
using Portfolio.Application.Extensions;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Articles;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class ArticleService(IUnitOfWork unitOfWork) : IArticleService
{
    public async Task<Result<IReadOnlyList<ArticleDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var articles = await unitOfWork.Articles.GetAllWithTranslationsAsync(cancellationToken);

        var dtos = articles.Select(a => MapToDto(a, language)).ToList();
        return Result<IReadOnlyList<ArticleDto>>.Success(dtos);
    }

    public async Task<Result<IReadOnlyList<ArticleDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var articles = await unitOfWork.Articles.GetActiveWithTranslationsAsync(cancellationToken);

        var dtos = articles.Select(a => MapToDto(a, language)).ToList();
        return Result<IReadOnlyList<ArticleDto>>.Success(dtos);
    }

    public async Task<Result<ArticleDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var article = await unitOfWork.Articles.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (article is null)
            return Result<ArticleDto>.Failure($"Article with id '{id}' was not found.");

        var language = ParseLanguage(languageCode);
        return Result<ArticleDto>.Success(MapToDto(article, language));
    }

    public async Task<Result<Guid>> CreateAsync(CreateArticleRequest request, CancellationToken cancellationToken = default)
    {
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Slug = request.Slug,
            Category = request.Category,
            Tags = request.Tags,
            CoverImageUrl = request.CoverImageUrl,
            PublishDate = request.PublishDate,
            ReadTimeMinutes = request.ReadTimeMinutes,
            IsPublished = request.IsPublished,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in request.Translations)
        {
            article.Translations.Add(new ArticleTranslation
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                Language = ParseLanguage(t.LanguageCode),
                Title = t.Title,
                Excerpt = t.Excerpt,
                Body = t.Body
            });
        }

        await unitOfWork.Articles.AddAsync(article, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(article.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateArticleRequest request, CancellationToken cancellationToken = default)
    {
        var article = await unitOfWork.Articles.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (article is null)
            return Result<bool>.Failure($"Article with id '{id}' was not found.");

        article.Slug = request.Slug;
        article.Category = request.Category;
        article.Tags = request.Tags;
        article.CoverImageUrl = request.CoverImageUrl;
        article.PublishDate = request.PublishDate;
        article.ReadTimeMinutes = request.ReadTimeMinutes;
        article.IsPublished = request.IsPublished;
        article.DisplayOrder = request.DisplayOrder;
        article.IsActive = request.IsActive;
        article.UpdatedAt = DateTime.UtcNow;

        article.Translations.SyncTranslations(
            request.Translations.Select(t => new ArticleTranslation
            {
                ArticleId = article.Id,
                Language = ParseLanguage(t.LanguageCode),
                Title = t.Title,
                Excerpt = t.Excerpt,
                Body = t.Body
            }).ToList(),
            (existing, incoming) =>
            {
                existing.Title = incoming.Title;
                existing.Excerpt = incoming.Excerpt;
                existing.Body = incoming.Body;
            });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await unitOfWork.Articles.GetByIdAsync(id, cancellationToken);
        if (article is null)
            return Result<bool>.Failure($"Article with id '{id}' was not found.");

        unitOfWork.Articles.Delete(article);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static Language ParseLanguage(string languageCode)
    {
        return Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language)
            ? language
            : Language.En;
    }

    private static ArticleDto MapToDto(Article article, Language language)
    {
        var translation = article.Translations.FirstOrDefault(t => t.Language == language);

        return new ArticleDto
        {
            Id = article.Id,
            Slug = article.Slug,
            Category = article.Category,
            Tags = article.Tags,
            CoverImageUrl = article.CoverImageUrl,
            PublishDate = article.PublishDate,
            ReadTimeMinutes = article.ReadTimeMinutes,
            IsPublished = article.IsPublished,
            DisplayOrder = article.DisplayOrder,
            IsActive = article.IsActive,
            CreatedAt = article.CreatedAt,
            Title = translation?.Title ?? string.Empty,
            Excerpt = translation?.Excerpt,
            Body = translation?.Body
        };
    }
}
