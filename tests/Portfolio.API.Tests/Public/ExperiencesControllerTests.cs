using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Experiences;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class ExperiencesControllerTests : IDisposable
{
    private readonly Mock<IExperienceService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ExperiencesControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IExperienceService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturn200WithExperiences()
    {
        var experiences = new List<ExperienceDto>
        {
            new() { Id = Guid.NewGuid(), CompanyName = "Company A", JobTitle = "Dev", CompanyLogo = "a.png", CompanyUrl = "https://a.com" }
        };
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ExperienceDto>>.Success(experiences));

        var response = await _client.GetAsync("/api/public/experiences");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ExperienceDto>>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_WithLangParam_ShouldPassLanguageToService()
    {
        _mockService.Setup(s => s.GetPublicAsync("ar", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ExperienceDto>>.Success(new List<ExperienceDto>()));

        var response = await _client.GetAsync("/api/public/experiences?lang=ar");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _mockService.Verify(s => s.GetPublicAsync("ar", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        var exp = new ExperienceDto { Id = id, CompanyName = "Company A", JobTitle = "Dev", CompanyLogo = "a.png", CompanyUrl = "https://a.com" };
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ExperienceDto>.Success(exp));

        var response = await _client.GetAsync($"/api/public/experiences/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExperienceDto>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ExperienceDto>.Failure("Experience not found."));

        var response = await _client.GetAsync($"/api/public/experiences/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
        body.Message.Should().Contain("not found");
    }
}
