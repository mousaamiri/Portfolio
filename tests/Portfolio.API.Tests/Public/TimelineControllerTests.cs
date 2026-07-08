using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Timeline;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class TimelineControllerTests : IDisposable
{
    private readonly Mock<ITimelineEntryService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TimelineControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITimelineEntryService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturn200WithEntries()
    {
        var entries = new List<TimelineEntryDto> { new() { Id = Guid.NewGuid(), Year = "2024", Title = "Built portfolio" } };
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<TimelineEntryDto>>.Success(entries));

        var response = await _client.GetAsync("/api/public/timeline");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<TimelineEntryDto>>>(JsonOptions);
        body!.Data![0].Title.Should().Be("Built portfolio");
    }
}
