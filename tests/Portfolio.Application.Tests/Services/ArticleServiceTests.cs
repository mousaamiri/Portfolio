using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Articles;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Articles;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class ArticleServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ArticleService _sut;

    public ArticleServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new ArticleService(_unitOfWorkMock.Object);
    }

    private static Article CreateArticle(Guid? id = null, string title = "Test Article", string language = "En")
    {
        var article = new Article
        {
            Id = id ?? Guid.NewGuid(),
            Slug = "test-article",
            Category = "Architecture",
            PublishDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            ReadTimeMinutes = 3,
            IsPublished = true,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        article.Translations.Add(new ArticleTranslation
        {
            Id = Guid.NewGuid(),
            ArticleId = article.Id,
            Language = Enum.Parse<Language>(language, ignoreCase: true),
            Title = title,
            Excerpt = "An excerpt",
            Body = "<p>Body</p>"
        });

        return article;
    }

    [Fact]
    public async Task GetPublicAsync_UsesActiveWithTranslationsRepositoryMethod()
    {
        var article = CreateArticle(title: "Public Article");
        _unitOfWorkMock.Setup(u => u.Articles.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Article> { article });

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value![0].Title.Should().Be("Public Article");
        _unitOfWorkMock.Verify(u => u.Articles.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_MapsAllPropertiesCorrectly()
    {
        var article = CreateArticle(title: "Mapped");
        _unitOfWorkMock.Setup(u => u.Articles.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Article> { article });

        var result = await _sut.GetAllAsync("en");

        var dto = result.Value![0];
        dto.Slug.Should().Be("test-article");
        dto.Category.Should().Be("Architecture");
        dto.ReadTimeMinutes.Should().Be(3);
        dto.IsPublished.Should().BeTrue();
        dto.Title.Should().Be("Mapped");
        dto.Excerpt.Should().Be("An excerpt");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsFailure()
    {
        _unitOfWorkMock.Setup(u => u.Articles.GetByIdWithTranslationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid(), "en");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateAsync_MapsRequestToEntityCorrectly()
    {
        Article? captured = null;
        _unitOfWorkMock.Setup(u => u.Articles.AddAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()))
            .Callback<Article, CancellationToken>((a, _) => captured = a)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new CreateArticleRequest
        {
            Slug = "new-post", Category = "Testing", ReadTimeMinutes = 5, IsPublished = true,
            Translations = [new() { LanguageCode = "en", Title = "New Post", Excerpt = "x", Body = "<p>y</p>" }]
        };

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        captured!.Slug.Should().Be("new-post");
        captured.ReadTimeMinutes.Should().Be(5);
        captured.Translations.Should().ContainSingle();
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ReturnsFailure()
    {
        _unitOfWorkMock.Setup(u => u.Articles.GetByIdWithTranslationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article?)null);

        var request = new UpdateArticleRequest { Slug = "x", IsActive = true, Translations = [] };
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request);

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesAndSaves()
    {
        var id = Guid.NewGuid();
        var article = CreateArticle(id: id);
        _unitOfWorkMock.Setup(u => u.Articles.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(article);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.DeleteAsync(id);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Articles.Delete(article), Times.Once);
    }
}
