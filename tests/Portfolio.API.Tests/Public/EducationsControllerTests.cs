using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Educations;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class EducationsControllerTests : IDisposable
{
    private readonly Mock<IEducationService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public EducationsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEducationService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturn200WithEducations()
    {
        var educations = new List<EducationDto>
        {
            new() { Id = Guid.NewGuid(), InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS", InstitutionLogo = "mit.png", InstitutionUrl = "https://mit.edu" }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<EducationDto>>.Success(educations));

        var response = await _client.GetAsync("/api/public/educations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<EducationDto>>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_WithLangParam_ShouldPassLanguageToService()
    {
        _mockService.Setup(s => s.GetAllAsync("fa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<EducationDto>>.Success(new List<EducationDto>()));

        var response = await _client.GetAsync("/api/public/educations?lang=fa");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _mockService.Verify(s => s.GetAllAsync("fa", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        var edu = new EducationDto { Id = id, InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS", InstitutionLogo = "mit.png", InstitutionUrl = "https://mit.edu" };
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EducationDto>.Success(edu));

        var response = await _client.GetAsync($"/api/public/educations/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<EducationDto>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EducationDto>.Failure("Education not found."));

        var response = await _client.GetAsync($"/api/public/educations/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
        body.Message.Should().Contain("not found");
    }
}
