using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Portfolio.Web.Controllers;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Controllers;

public class AboutControllerTests
{
    private readonly Mock<IPortfolioApiClient> _api = new();
    private readonly AboutController _sut;

    public AboutControllerTests()
    {
        _api.Setup(a => a.GetSkillsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new SkillApiDto { Name = "C#", Category = "Backend", Proficiency = 85, IconUrl = "csharp.svg" }]);
        _api.Setup(a => a.GetEducationsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new EducationApiDto { InstitutionName = "State University", Degree = "BSc", FieldOfStudy = "CS", Gpa = 18.2 }]);
        _api.Setup(a => a.GetTimelineAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new TimelineEntryApiDto { Year = "2024", Icon = "rocket", Title = "Built portfolio", Description = "d" }]);
        _api.Setup(a => a.GetInterestsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new InterestApiDto { Icon = "code", Label = "Coding" }]);
        _api.Setup(a => a.GetStatsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new StatCounterApiDto { Icon = "folder", Label = "Projects", CountTarget = 48, Suffix = "+" }]);

        _sut = new AboutController(_api.Object);
    }

    private async Task<AboutViewModel> InvokeAsync()
    {
        var result = await _sut.Index(null, CancellationToken.None) as ViewResult;
        return (AboutViewModel)result!.Model!;
    }

    [Fact]
    public async Task Index_ReturnsViewWithAboutViewModel()
    {
        var result = await _sut.Index(null, CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        (await InvokeAsync()).Should().BeOfType<AboutViewModel>();
    }

    [Fact]
    public async Task Index_SkillsComeFromApi()
    {
        var model = await InvokeAsync();

        model.Skills.Should().ContainSingle();
        model.Skills[0].Name.Should().Be("C#");
        model.Skills[0].IconClass.Should().Be("csharp.svg");
        _api.Verify(a => a.GetSkillsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Index_EducationComesFromApi()
    {
        var model = await InvokeAsync();

        model.Education.Should().ContainSingle();
        model.Education[0].InstitutionName.Should().Be("State University");
        _api.Verify(a => a.GetEducationsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Index_JourneyComesFromApi()
    {
        var model = await InvokeAsync();

        model.Journey.Should().ContainSingle();
        model.Journey[0].Year.Should().Be("2024");
        model.Journey[0].Title.Should().Be("Built portfolio");
        _api.Verify(a => a.GetTimelineAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Index_InterestsComeFromApi()
    {
        var model = await InvokeAsync();

        model.Interests.Should().ContainSingle();
        model.Interests[0].Label.Should().Be("Coding");
        model.Interests[0].Icon.Should().Be("code");
        _api.Verify(a => a.GetInterestsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Index_FootprintComesFromApi()
    {
        var model = await InvokeAsync();

        model.Footprint.Should().ContainSingle();
        model.Footprint[0].Label.Should().Be("Projects");
        model.Footprint[0].CountTarget.Should().Be(48);
        _api.Verify(a => a.GetStatsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Index_MockedSectionsStillPopulated()
    {
        var model = await InvokeAsync();

        // Endorsements have no backend entity yet (E2 skipped), so they remain
        // mock-backed and non-empty.
        model.Endorsements.Should().NotBeEmpty();
    }
}
