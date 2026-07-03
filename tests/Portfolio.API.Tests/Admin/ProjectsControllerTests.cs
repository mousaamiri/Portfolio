using System.Net;
using System.Net.Http.Headers;
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

namespace Portfolio.API.Tests.Admin;

public class ProjectsControllerTests : IDisposable
{
    private readonly Mock<IProjectService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly string _token = TestTokenHelper.GenerateToken();

    public ProjectsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IProjectService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    public void Dispose() => _factory.Dispose();

    private HttpClient CreateUnauthenticatedClient() => _factory.CreateClient();

    [Fact]
    public async Task GetAll_WithToken_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ProjectDto>>.Success(new List<ProjectDto>()));

        var response = await _client.GetAsync("/api/admin/projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProjectDto>>>(JsonOptions);
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn401WithApiResponse()
    {
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/admin/projects");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
        body.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        var project = new ProjectDto { Id = id, Title = "Test", ImageUrl = "img.png" };
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Success(project));

        var response = await _client.GetAsync($"/api/admin/projects/{id}");

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

        var response = await _client.GetAsync($"/api/admin/projects/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Create_ValidRequest_ShouldReturn201()
    {
        var newId = Guid.NewGuid();
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateProjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(newId));

        var request = new CreateProjectRequest
        {
            ImageUrl = "img.png",
            DisplayOrder = 1,
            Translations = [new() { LanguageCode = "en", Title = "Test" }]
        };

        var response = await _client.PostAsJsonAsync("/api/admin/projects", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data.Should().Be(newId);
    }

    [Fact]
    public async Task Create_InvalidRequest_ShouldReturn400WithErrors()
    {
        var request = new { ImageUrl = "", Translations = new List<object>() };

        var response = await _client.PostAsJsonAsync("/api/admin/projects", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
        body.Errors.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Update_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateProjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var request = new UpdateProjectRequest
        {
            ImageUrl = "img.png",
            DisplayOrder = 1,
            IsActive = true,
            Translations = [new() { LanguageCode = "en", Title = "Updated" }]
        };

        var response = await _client.PutAsJsonAsync($"/api/admin/projects/{id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(JsonOptions);
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Update_NonExistingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateProjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Project not found."));

        var request = new UpdateProjectRequest
        {
            ImageUrl = "img.png",
            DisplayOrder = 1,
            IsActive = true,
            Translations = [new() { LanguageCode = "en", Title = "Updated" }]
        };

        var response = await _client.PutAsJsonAsync($"/api/admin/projects/{id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var response = await _client.DeleteAsync($"/api/admin/projects/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(JsonOptions);
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_NonExistingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Project not found."));

        var response = await _client.DeleteAsync($"/api/admin/projects/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}