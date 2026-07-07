using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Portfolio.Web.Controllers;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<IPortfolioApiClient> _api = new();
    private readonly HomeController _sut;

    public HomeControllerTests()
    {
        _api.Setup(a => a.GetProjectsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ProjectApiDto { Id = Guid.NewGuid(), Title = "Project 1", ThumbnailUrl = "p.png" }]);
        _api.Setup(a => a.GetSkillsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new SkillApiDto { Id = Guid.NewGuid(), Name = "C#", IconUrl = "csharp.svg" }]);
        _api.Setup(a => a.GetExperiencesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ExperienceApiDto { Id = Guid.NewGuid(), CompanyName = "Acme", JobTitle = "Engineer" }]);
        _api.Setup(a => a.GetEducationsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new EducationApiDto { Id = Guid.NewGuid(), InstitutionName = "MIT", Degree = "BSc" }]);

        _sut = new HomeController(_api.Object);
    }

    private async Task<HomeViewModel> InvokeIndexAsync()
    {
        var result = await _sut.Index(null, CancellationToken.None) as ViewResult;
        return (HomeViewModel)result!.Model!;
    }

    #region Index

    [Fact]
    public async Task Index_Always_ReturnsViewResult()
    {
        var result = await _sut.Index(null, CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Index_Always_ReturnsViewWithHomeViewModel()
    {
        var result = await _sut.Index(null, CancellationToken.None) as ViewResult;

        result!.Model.Should().BeOfType<HomeViewModel>();
    }

    [Fact]
    public async Task Index_Model_ContainsProjects()
    {
        var model = await InvokeIndexAsync();

        model.Projects.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Index_Model_ContainsSkills()
    {
        var model = await InvokeIndexAsync();

        model.Skills.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Index_Model_ContainsExperiences()
    {
        var model = await InvokeIndexAsync();

        model.Experiences.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Index_Model_ContainsEducations()
    {
        var model = await InvokeIndexAsync();

        model.Educations.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Index_Model_HasFullName()
    {
        var model = await InvokeIndexAsync();

        model.FullName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Index_Model_HasJobTitle()
    {
        var model = await InvokeIndexAsync();

        model.JobTitle.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Index_MapsApiProjectDataOntoViewModel()
    {
        var model = await InvokeIndexAsync();

        model.Projects[0].Title.Should().Be("Project 1");
        model.Projects[0].ImageUrl.Should().Be("p.png");
    }

    #endregion
}
