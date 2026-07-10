using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class UiTranslationsControllerTests : IDisposable
{
    private readonly Mock<IUiTranslationService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public UiTranslationsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUiTranslationService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetMap_ShouldReturn200WithKeyValueMap()
    {
        var map = new Dictionary<string, string> { ["nav.home"] = "خانه", ["nav.about"] = "درباره من" };
        _mockService.Setup(s => s.GetMapAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyDictionary<string, string>>.Success(map));

        var response = await _client.GetAsync("/api/public/ui-translations?lang=fa");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<string, string>>>(JsonOptions);
        body!.Data.Should().HaveCount(2);
        body.Data!["nav.home"].Should().Be("خانه");
    }

    [Fact]
    public async Task GetMap_English_ShouldReturn200WithDbMap()
    {
        var map = new Dictionary<string, string> { ["nav.home"] = "Home" };
        _mockService.Setup(s => s.GetMapAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyDictionary<string, string>>.Success(map));

        var response = await _client.GetAsync("/api/public/ui-translations?lang=en");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<string, string>>>(JsonOptions);
        body!.Data!["nav.home"].Should().Be("Home");
    }
}
