using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Portfolio.Web.Controllers;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Controllers;

public class ExperienceControllerTests
{
    private readonly Mock<IPortfolioApiClient> _api = new();
    private readonly ExperienceController _sut;

    public ExperienceControllerTests()
    {
        // Real client never returns null; default all reads to empty so each test
        // only sets up what it asserts.
        _api.Setup(a => a.GetExperiencesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _api.Setup(a => a.GetImpactMetricsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _api.Setup(a => a.GetPrinciplesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _api.Setup(a => a.GetProficienciesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        _sut = new ExperienceController(_api.Object);
    }

    private async Task<ExperiencePageViewModel> InvokeAsync()
    {
        var result = await _sut.Index(null, CancellationToken.None) as ViewResult;
        return (ExperiencePageViewModel)result!.Model!;
    }

    [Fact]
    public async Task Index_ProfessionalHistoryComesFromApi()
    {
        _api.Setup(a => a.GetExperiencesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ExperienceApiDto { CompanyName = "Acme", JobTitle = "Engineer", StartDate = new DateTime(2022, 1, 1) }]);

        var model = await InvokeAsync();

        model.ProfessionalHistory.Should().ContainSingle();
        model.ProfessionalHistory[0].CompanyName.Should().Be("Acme");
        _api.Verify(a => a.GetExperiencesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Index_WhenApiEmpty_ProfessionalHistoryEmptyForFallback()
    {
        _api.Setup(a => a.GetExperiencesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var model = await InvokeAsync();

        model.ProfessionalHistory.Should().BeEmpty();
        // The static fallback role block is still available on the model.
        model.JobTitle.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Index_MetricsPrinciplesProficiencyComeFromApi()
    {
        _api.Setup(a => a.GetExperiencesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _api.Setup(a => a.GetImpactMetricsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ImpactMetricApiDto { Value = "99.9%", Tag = "UPTIME", Color = "pink" }]);
        _api.Setup(a => a.GetPrinciplesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new PrincipleApiDto { Title = "Scale-First", Description = "d" }]);
        _api.Setup(a => a.GetProficienciesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ProficiencyGroupApiDto { Title = "MASTERY", Color = "amber", Items = "Java 21, Spring Boot" }]);

        var model = await InvokeAsync();

        model.Metrics.Should().ContainSingle();
        model.Metrics[0].Value.Should().Be("99.9%");
        model.Principles.Should().ContainSingle();
        model.Principles[0].Title.Should().Be("Scale-First");
        model.Proficiency.Should().ContainSingle();
        model.Proficiency[0].Items.Should().BeEquivalentTo("Java 21", "Spring Boot");
        _api.Verify(a => a.GetImpactMetricsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _api.Verify(a => a.GetPrinciplesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _api.Verify(a => a.GetProficienciesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Index_MockedSectionsStillPopulated()
    {
        _api.Setup(a => a.GetExperiencesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var model = await InvokeAsync();

        // CV-style Education / Stack have no backend entity yet, so they stay mocked.
        model.Education.Should().NotBeEmpty();
        model.Stack.Should().NotBeEmpty();
    }
}
