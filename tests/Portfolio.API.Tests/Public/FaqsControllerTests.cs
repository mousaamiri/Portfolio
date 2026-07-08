using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Faqs;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class FaqsControllerTests : IDisposable
{
    private readonly Mock<IFaqService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public FaqsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFaqService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturn200WithFaqs()
    {
        var faqs = new List<FaqDto> { new() { Id = Guid.NewGuid(), Question = "Remote?", Answer = "Yes" } };
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<FaqDto>>.Success(faqs));

        var response = await _client.GetAsync("/api/public/faqs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<FaqDto>>>(JsonOptions);
        body!.Data.Should().HaveCount(1);
        body.Data![0].Question.Should().Be("Remote?");
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<FaqDto>>.Success(new List<FaqDto>()));

        var response = await _client.GetAsync("/api/public/faqs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
