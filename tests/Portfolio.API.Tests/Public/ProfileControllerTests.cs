using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Profiles;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class ProfileControllerTests : IDisposable
{
    private readonly Mock<IProfileService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ProfileControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IProfileService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Get_WithProfile_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto>.Success(new ProfileDto { FullName = "Mousa", Email = "me@x.com" }));

        var response = await _client.GetAsync("/api/public/profile");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ProfileDto>>(JsonOptions);
        body!.Data!.FullName.Should().Be("Mousa");
    }

    [Fact]
    public async Task Get_WhenNoProfile_ShouldReturn404()
    {
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto>.Failure("No profile"));

        var response = await _client.GetAsync("/api/public/profile");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
