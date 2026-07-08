using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Interests;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class InterestsControllerTests : IDisposable
{
    private readonly Mock<IInterestService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public InterestsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IInterestService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturn200WithInterests()
    {
        var interests = new List<InterestDto> { new() { Id = Guid.NewGuid(), Icon = "code", Label = "Coding" } };
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<InterestDto>>.Success(interests));

        var response = await _client.GetAsync("/api/public/interests");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<InterestDto>>>(JsonOptions);
        body!.Data![0].Label.Should().Be("Coding");
    }
}
