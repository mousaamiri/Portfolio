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

namespace Portfolio.API.Tests.Public;

public class ProjectsControllerTests : IDisposable
{
    private readonly Mock<IProjectService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ProjectsControllerTests()
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
    public async Task GetAll_ShouldReturn200WithProjects()
    {
        var projects = new List<ProjectDto>
        {
            new() { Id = Guid.NewGuid(), Title = "Project 1", ThumbnailUrl = "img1.png" },
            new() { Id = Guid.NewGuid(), Title = "Project 2", ThumbnailUrl = "img2.png" }
        };
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ProjectDto>>.Success(projects));

        var response = await _client.GetAsync("/api/public/projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProjectDto>>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data.Should().HaveCount(2);
        body.Data![0].Title.Should().Be("Project 1");
    }

    [Fact]
    public async Task GetAll_WithLangParam_ShouldPassLanguageToService()
    {
        _mockService.Setup(s => s.GetPublicAsync("fa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ProjectDto>>.Success(new List<ProjectDto>()));

        var response = await _client.GetAsync("/api/public/projects?lang=fa");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _mockService.Verify(s => s.GetPublicAsync("fa", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        var project = new ProjectDto { Id = id, Title = "Test", ThumbnailUrl = "img.png" };
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Success(project));

        var response = await _client.GetAsync($"/api/public/projects/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ProjectDto>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Failure("Project not found."));

        var response = await _client.GetAsync($"/api/public/projects/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
        body.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ProjectDto>>.Success(new List<ProjectDto>()));

        var response = await _client.GetAsync("/api/public/projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
