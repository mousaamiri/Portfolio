using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Testimonials;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class TestimonialsControllerTests : IDisposable
{
    private readonly Mock<ITestimonialService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TestimonialsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var d = services.SingleOrDefault(x => x.ServiceType == typeof(ITestimonialService));
            if (d != null) services.Remove(d);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturn200WithTestimonials()
    {
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<TestimonialDto>>.Success(
                new List<TestimonialDto> { new() { Name = "Jane", Quote = "Great" } }));

        var response = await _client.GetAsync("/api/public/testimonials");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<TestimonialDto>>>(JsonOptions);
        body!.Data![0].Name.Should().Be("Jane");
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ShouldReturn200EmptyList()
    {
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<TestimonialDto>>.Success(new List<TestimonialDto>()));

        var response = await _client.GetAsync("/api/public/testimonials");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<TestimonialDto>>>(JsonOptions);
        body!.Data.Should().BeEmpty();
    }
}
