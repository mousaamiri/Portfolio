using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities.Articles;
using Portfolio.Domain.Enums;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure.Tests.Repositories;

public class ArticleRepositoryTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private static Article CreateArticle(DateTime publishDate) => new()
    {
        Id = Guid.NewGuid(),
        Slug = Guid.NewGuid().ToString(),
        PublishDate = publishDate,
        ReadTimeMinutes = 2,
        IsPublished = true,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    [Fact]
    public async Task GetActiveWithTranslationsAsync_ReturnsOnlyActivePublishedOrderedByPublishDateDescending()
    {
        var dbName = Guid.NewGuid().ToString();

        var older = CreateArticle(new DateTime(2025, 1, 1));
        var newer = CreateArticle(new DateTime(2025, 6, 1));
        var inactive = CreateArticle(new DateTime(2025, 7, 1));
        inactive.IsActive = false;
        var unpublished = CreateArticle(new DateTime(2025, 8, 1));
        unpublished.IsPublished = false;

        newer.Translations.Add(new ArticleTranslation
        {
            Id = Guid.NewGuid(), ArticleId = newer.Id, Language = Language.En, Title = "Newer"
        });

        await using (var context = CreateContext(dbName))
        {
            context.Articles.AddRange(older, newer, inactive, unpublished);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ArticleRepository(context);
            var result = await repo.GetActiveWithTranslationsAsync();

            result.Should().HaveCount(2);
            result.Should().OnlyContain(a => a.IsActive && a.IsPublished);
            result[0].Id.Should().Be(newer.Id);
            result[0].Translations.Should().ContainSingle();
            result[1].Id.Should().Be(older.Id);
        }
    }

    [Fact]
    public async Task GetByIdWithTranslationsAsync_ShouldEagerLoadTranslations()
    {
        var dbName = Guid.NewGuid().ToString();
        var article = CreateArticle(new DateTime(2025, 3, 1));
        article.Translations.Add(new ArticleTranslation
        {
            Id = Guid.NewGuid(), ArticleId = article.Id, Language = Language.En, Title = "T", Body = "<p>b</p>"
        });

        await using (var context = CreateContext(dbName))
        {
            context.Articles.Add(article);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ArticleRepository(context);
            var result = await repo.GetByIdWithTranslationsAsync(article.Id);

            result.Should().NotBeNull();
            result!.Translations.Should().ContainSingle();
            result.Translations.First().Title.Should().Be("T");
        }
    }
}
