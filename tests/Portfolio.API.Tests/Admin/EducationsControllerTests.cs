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
using Portfolio.Application.DTOs.Educations;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Admin;

public class EducationsControllerTests : IDisposable
{
    private readonly Mock<IEducationService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly string _token = TestTokenHelper.GenerateToken();

    public EducationsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEducationService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_WithToken_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<EducationDto>>.Success(new List<EducationDto>()));

        var response = await _client.GetAsync("/api/admin/educations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<EducationDto>>>(JsonOptions);
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/admin/educations");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EducationDto>.Success(new EducationDto
            { Id = id, InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS", InstitutionLogo = "mit.png", InstitutionUrl = "https://mit.edu" }));

        var response = await _client.GetAsync($"/api/admin/educations/{id}");

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

        var response = await _client.GetAsync($"/api/admin/educations/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_ValidRequest_ShouldReturn201()
    {
        var newId = Guid.NewGuid();
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateEducationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(newId));

        var request = new CreateEducationRequest
        {
            InstitutionLogo = "mit.png", InstitutionUrl = "https://mit.edu",
            StartDate = DateTime.UtcNow.AddYears(-4),
            Translations = [new() { LanguageCode = "en", InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS" }]
        };

        var response = await _client.PostAsJsonAsync("/api/admin/educations", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data.Should().Be(newId);
    }

    [Fact]
    public async Task Create_InvalidRequest_ShouldReturn400WithErrors()
    {
        var request = new { InstitutionLogo = "", InstitutionUrl = "", Translations = new List<object>() };
        var response = await _client.PostAsJsonAsync("/api/admin/educations", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
        body.Errors.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Update_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateEducationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var request = new UpdateEducationRequest
        {
            InstitutionLogo = "mit.png", InstitutionUrl = "https://mit.edu",
            StartDate = DateTime.UtcNow.AddYears(-4), IsActive = true,
            Translations = [new() { LanguageCode = "en", InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS" }]
        };

        var response = await _client.PutAsJsonAsync($"/api/admin/educations/{id}", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_NonExistingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateEducationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Education not found."));

        var request = new UpdateEducationRequest
        {
            InstitutionLogo = "mit.png", InstitutionUrl = "https://mit.edu",
            StartDate = DateTime.UtcNow.AddYears(-4), IsActive = true,
            Translations = [new() { LanguageCode = "en", InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS" }]
        };

        var response = await _client.PutAsJsonAsync($"/api/admin/educations/{id}", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var response = await _client.DeleteAsync($"/api/admin/educations/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_NonExistingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Education not found."));

        var response = await _client.DeleteAsync($"/api/admin/educations/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}