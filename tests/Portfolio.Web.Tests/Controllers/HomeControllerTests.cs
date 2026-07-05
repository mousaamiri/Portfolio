using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Controllers;
using Portfolio.Web.Models.ViewModels;

namespace Portfolio.Web.Tests.Controllers;

public class HomeControllerTests
{
    private readonly HomeController _sut = new();

    #region Index

    [Fact]
    public void Index_Always_ReturnsViewResult()
    {
        var result = _sut.Index();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Index_Always_ReturnsViewWithHomeViewModel()
    {
        var result = _sut.Index() as ViewResult;

        result!.Model.Should().BeOfType<HomeViewModel>();
    }

    [Fact]
    public void Index_Model_ContainsProjects()
    {
        var result = _sut.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        model!.Projects.Should().NotBeEmpty();
    }

    [Fact]
    public void Index_Model_ContainsSkills()
    {
        var result = _sut.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        model!.Skills.Should().NotBeEmpty();
    }

    [Fact]
    public void Index_Model_ContainsExperiences()
    {
        var result = _sut.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        model!.Experiences.Should().NotBeEmpty();
    }

    [Fact]
    public void Index_Model_ContainsEducations()
    {
        var result = _sut.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        model!.Educations.Should().NotBeEmpty();
    }

    [Fact]
    public void Index_Model_HasFullName()
    {
        var result = _sut.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        model!.FullName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Index_Model_HasJobTitle()
    {
        var result = _sut.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        model!.JobTitle.Should().NotBeNullOrWhiteSpace();
    }

    #endregion
}
