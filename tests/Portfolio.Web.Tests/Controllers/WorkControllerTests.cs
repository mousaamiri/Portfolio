using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Portfolio.Web.Controllers;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Controllers;

public class WorkControllerTests
{
    private readonly Mock<IPortfolioApiClient> _api = new();
    private readonly WorkController _sut;
    private readonly Guid _id = Guid.NewGuid();

    public WorkControllerTests()
    {
        _api.Setup(a => a.GetProjectsAsync("en", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ProjectApiDto
            {
                Id = _id, DisplayOrder = 1, Title = "GitGlance",
                ShortDescription = "GitHub visualizer", Description = "A dashboard.",
                Technologies = "React, Java", SourceCodeUrl = "https://github.com/x"
            }]);
        _api.Setup(a => a.GetProjectsAsync("fa", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ProjectApiDto
            {
                Id = _id, DisplayOrder = 1, Title = "گیت‌گلنس",
                ShortDescription = "مصورساز گیت‌هاب", Description = "یک داشبورد."
            }]);

        _sut = new WorkController(_api.Object);
    }

    private async Task<List<ProjectViewModel>> InvokeAsync()
    {
        var result = await _sut.Index(CancellationToken.None) as ViewResult;
        return (List<ProjectViewModel>)result!.Model!;
    }

    [Fact]
    public async Task Index_ReturnsViewWithProjectList()
    {
        var result = await _sut.Index(CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        (await InvokeAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task Index_MergesEnglishAndFarsiTranslations()
    {
        var model = await InvokeAsync();

        model.Should().ContainSingle();
        model[0].NameEn.Should().Be("GitGlance");
        model[0].NameFa.Should().Be("گیت‌گلنس");
        model[0].SubtitleEn.Should().Be("GitHub visualizer");
        model[0].SubtitleFa.Should().Be("مصورساز گیت‌هاب");
    }

    [Fact]
    public async Task Index_FetchesBothLanguages()
    {
        await InvokeAsync();

        _api.Verify(a => a.GetProjectsAsync("en", It.IsAny<CancellationToken>()), Times.Once);
        _api.Verify(a => a.GetProjectsAsync("fa", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Index_AssignsTechPillColorsFromFrontendMap()
    {
        var model = await InvokeAsync();

        var java = model[0].Techs.Single(t => t.Name == "Java");
        java.Color.Should().Be("#f0a63b");
    }

    [Fact]
    public async Task Index_FarsiMissing_FallsBackToEnglish()
    {
        _api.Setup(a => a.GetProjectsAsync("fa", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var model = await InvokeAsync();

        model[0].NameFa.Should().Be("GitGlance");
    }
}
