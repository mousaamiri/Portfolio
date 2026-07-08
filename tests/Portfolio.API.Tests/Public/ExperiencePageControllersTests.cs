using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.ImpactMetrics;
using Portfolio.Application.DTOs.Principles;
using Portfolio.Application.DTOs.Proficiencies;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Public;

public class ExperiencePageControllersTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task ImpactMetrics_GetAll_ShouldReturn200()
    {
        var mock = new Mock<IImpactMetricService>();
        mock.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ImpactMetricDto>>.Success(
                new List<ImpactMetricDto> { new() { Value = "99.9%", Tag = "UPTIME" } }));

        using var factory = new MockWebApplicationFactory(services =>
        {
            var d = services.SingleOrDefault(x => x.ServiceType == typeof(IImpactMetricService));
            if (d != null) services.Remove(d);
            services.AddScoped(_ => mock.Object);
        });

        var response = await factory.CreateClient().GetAsync("/api/public/impact-metrics");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ImpactMetricDto>>>(JsonOptions);
        body!.Data![0].Value.Should().Be("99.9%");
    }

    [Fact]
    public async Task Principles_GetAll_ShouldReturn200()
    {
        var mock = new Mock<IPrincipleService>();
        mock.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<PrincipleDto>>.Success(
                new List<PrincipleDto> { new() { Title = "Scale-First" } }));

        using var factory = new MockWebApplicationFactory(services =>
        {
            var d = services.SingleOrDefault(x => x.ServiceType == typeof(IPrincipleService));
            if (d != null) services.Remove(d);
            services.AddScoped(_ => mock.Object);
        });

        var response = await factory.CreateClient().GetAsync("/api/public/principles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<PrincipleDto>>>(JsonOptions);
        body!.Data![0].Title.Should().Be("Scale-First");
    }

    [Fact]
    public async Task Proficiencies_GetAll_ShouldReturn200()
    {
        var mock = new Mock<IProficiencyGroupService>();
        mock.Setup(s => s.GetPublicAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ProficiencyGroupDto>>.Success(
                new List<ProficiencyGroupDto> { new() { Title = "MASTERY", Items = "Java 21, Spring Boot" } }));

        using var factory = new MockWebApplicationFactory(services =>
        {
            var d = services.SingleOrDefault(x => x.ServiceType == typeof(IProficiencyGroupService));
            if (d != null) services.Remove(d);
            services.AddScoped(_ => mock.Object);
        });

        var response = await factory.CreateClient().GetAsync("/api/public/proficiencies");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProficiencyGroupDto>>>(JsonOptions);
        body!.Data![0].Items.Should().Be("Java 21, Spring Boot");
    }
}
