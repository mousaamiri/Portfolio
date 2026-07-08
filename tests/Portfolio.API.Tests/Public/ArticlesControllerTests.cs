using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Articles;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class ArticlesControllerTests : IDisposable
{
    private readonly Mock<IArticleService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ArticlesControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IArticleService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturn200WithArticles()
    {
        var articles = new List<ArticleDto>
        {
            new() { Id = Guid.NewGuid(), Title = "Article 1", Slug = "article-1" },
            new() { Id = Guid.NewGuid(), Title = "Article 2", Slug = "article-2" }
        };
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ArticleDto>>.Success(articles));

        var response = await _client.GetAsync("/api/public/articles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ArticleDto>>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data.Should().HaveCount(2);
        body.Data![0].Title.Should().Be("Article 1");
    }

    [Fact]
    public async Task GetAll_WithLangParam_ShouldPassLanguageToService()
    {
        _mockService.Setup(s => s.GetPublicAsync("fa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ArticleDto>>.Success(new List<ArticleDto>()));

        var response = await _client.GetAsync("/api/public/articles?lang=fa");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _mockService.Verify(s => s.GetPublicAsync("fa", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        var article = new ArticleDto { Id = id, Title = "Test", Slug = "test" };
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ArticleDto>.Success(article));

        var response = await _client.GetAsync($"/api/public/articles/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ArticleDto>>(JsonOptions);
        body!.Data!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ArticleDto>.Failure("Article not found."));

        var response = await _client.GetAsync($"/api/public/articles/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
