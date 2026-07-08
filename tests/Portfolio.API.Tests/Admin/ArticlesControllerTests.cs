using System.Net;
using System.Net.Http.Headers;
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

namespace Portfolio.API.Tests.Admin;

public class ArticlesControllerTests : IDisposable
{
    private readonly Mock<IArticleService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly string _token = TestTokenHelper.GenerateToken();

    public ArticlesControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IArticleService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    public void Dispose() => _factory.Dispose();

    private HttpClient CreateUnauthenticatedClient() => _factory.CreateClient();

    [Fact]
    public async Task GetAll_WithToken_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ArticleDto>>.Success(new List<ArticleDto>()));

        var response = await _client.GetAsync("/api/admin/articles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn401()
    {
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/admin/articles");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_ValidRequest_ShouldReturn201()
    {
        var newId = Guid.NewGuid();
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateArticleRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(newId));

        var request = new CreateArticleRequest
        {
            Slug = "my-article",
            ReadTimeMinutes = 3,
            Translations = [new() { LanguageCode = "en", Title = "Test" }]
        };

        var response = await _client.PostAsJsonAsync("/api/admin/articles", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(JsonOptions);
        body!.Data.Should().Be(newId);
    }

    [Fact]
    public async Task Create_InvalidRequest_ShouldReturn400WithErrors()
    {
        var request = new { Slug = "", Translations = new List<object>() };

        var response = await _client.PostAsJsonAsync("/api/admin/articles", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Errors.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Update_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateArticleRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var request = new UpdateArticleRequest
        {
            Slug = "my-article",
            ReadTimeMinutes = 3,
            IsActive = true,
            Translations = [new() { LanguageCode = "en", Title = "Updated" }]
        };

        var response = await _client.PutAsJsonAsync($"/api/admin/articles/{id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var response = await _client.DeleteAsync($"/api/admin/articles/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
