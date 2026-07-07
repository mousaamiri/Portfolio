using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Skills;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class SkillsControllerTests : IDisposable
{
    private readonly Mock<ISkillService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SkillsControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISkillService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturn200WithSkills()
    {
        var skills = new List<SkillDto>
        {
            new() { Id = Guid.NewGuid(), Name = "C#", IconUrl = "csharp.png" },
            new() { Id = Guid.NewGuid(), Name = "TypeScript", IconUrl = "ts.png" }
        };
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<SkillDto>>.Success(skills));

        var response = await _client.GetAsync("/api/public/skills");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<SkillDto>>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_WithLangParam_ShouldPassLanguageToService()
    {
        _mockService.Setup(s => s.GetPublicAsync("fa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<SkillDto>>.Success(new List<SkillDto>()));

        var response = await _client.GetAsync("/api/public/skills?lang=fa");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _mockService.Verify(s => s.GetPublicAsync("fa", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        var skill = new SkillDto { Id = id, Name = "C#", IconUrl = "csharp.png" };
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SkillDto>.Success(skill));

        var response = await _client.GetAsync($"/api/public/skills/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SkillDto>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SkillDto>.Failure("Skill not found."));

        var response = await _client.GetAsync($"/api/public/skills/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
        body.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<SkillDto>>.Success(new List<SkillDto>()));

        var response = await _client.GetAsync("/api/public/skills");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
