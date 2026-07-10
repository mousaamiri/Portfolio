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
using Portfolio.Application.DTOs.UiTranslations;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Admin;

public class UiTranslationsControllerTests : IDisposable
{
    private readonly Mock<IUiTranslationService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly string _token = TestTokenHelper.GenerateToken();

    public UiTranslationsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUiTranslationService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_WithToken_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<UiTranslationDto>>.Success(new List<UiTranslationDto>
            {
                new() { Id = Guid.NewGuid(), Key = "nav.home", Language = "fa", Value = "خانه", IsActive = true }
            }));

        var response = await _client.GetAsync("/api/admin/ui-translations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<UiTranslationDto>>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data.Should().ContainSingle();
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/admin/ui-translations");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_ValidRequest_ShouldReturn201()
    {
        var newId = Guid.NewGuid();
        _mockService.Setup(s => s.CreateAsync(It.IsAny<UpsertUiTranslationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(newId));

        var response = await _client.PostAsJsonAsync("/api/admin/ui-translations",
            new UpsertUiTranslationRequest { Key = "nav.home", LanguageCode = "fa", Value = "خانه" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DuplicateKey_ShouldReturn400()
    {
        _mockService.Setup(s => s.CreateAsync(It.IsAny<UpsertUiTranslationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Failure("A translation for key 'nav.home' in 'Fa' already exists."));

        var response = await _client.PostAsJsonAsync("/api/admin/ui-translations",
            new UpsertUiTranslationRequest { Key = "nav.home", LanguageCode = "fa", Value = "خانه" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.UpdateAsync(id, It.IsAny<UpsertUiTranslationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var response = await _client.PutAsJsonAsync($"/api/admin/ui-translations/{id}",
            new UpsertUiTranslationRequest { Key = "nav.home", LanguageCode = "fa", Value = "خانهٔ نو" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var response = await _client.DeleteAsync($"/api/admin/ui-translations/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_WithoutToken_ShouldReturn401()
    {
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/admin/ui-translations/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
