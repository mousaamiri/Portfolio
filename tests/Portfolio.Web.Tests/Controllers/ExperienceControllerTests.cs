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
    public async Task Index_MockedSectionsStillPopulated()
    {
        _api.Setup(a => a.GetExperiencesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var model = await InvokeAsync();

        model.Metrics.Should().NotBeEmpty();
        model.Principles.Should().NotBeEmpty();
        model.Proficiency.Should().NotBeEmpty();
        model.Education.Should().NotBeEmpty();
    }
}
