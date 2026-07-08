using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Stats;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class StatsControllerTests : IDisposable
{
    private readonly Mock<IStatCounterService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public StatsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IStatCounterService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturn200WithStats()
    {
        var stats = new List<StatCounterDto> { new() { Id = Guid.NewGuid(), Label = "Commits", CountTarget = 1500 } };
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<StatCounterDto>>.Success(stats));

        var response = await _client.GetAsync("/api/public/stats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<StatCounterDto>>>(JsonOptions);
        body!.Data![0].Label.Should().Be("Commits");
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<StatCounterDto>>.Success(new List<StatCounterDto>()));

        var response = await _client.GetAsync("/api/public/stats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
