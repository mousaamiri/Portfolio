using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Common;

public class ExceptionHandlingMiddlewareTests : IDisposable
{
    private readonly Mock<IProjectService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ExceptionHandlingMiddlewareTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IProjectService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task UnhandledException_ShouldReturn500WithApiResponse()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        var response = await _client.GetAsync("/api/public/projects");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body.Should().NotBeNull();
        body!.Success.Should().BeFalse();
        body.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UnhandledException_ShouldNotLeakStackTrace()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Sensitive database error details"));

        var response = await _client.GetAsync("/api/public/projects");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotContain("Sensitive database error details");
        content.Should().NotContain("StackTrace");
        content.Should().NotContain("InvalidOperationException");
    }

    [Fact]
    public async Task UnhandledException_ResponseShouldBeValidApiResponseJson()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        var response = await _client.GetAsync("/api/public/projects");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");

        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
        body.Message.Should().Be("An unexpected error occurred. Please try again later.");
    }
}