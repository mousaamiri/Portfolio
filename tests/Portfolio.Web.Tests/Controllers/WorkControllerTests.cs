using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Portfolio.Web.Controllers;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
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
                ShortDescription = "مصورساز گیت‌هاب", Description = "یک داشبورد.",
                Technologies = "React, Java"
            }]);

        _sut = new WorkController(_api.Object);
    }

    /// <summary>Attaches an HttpContext (optionally with a language cookie) so the
    /// controller's cookie-based language resolution works in tests.</summary>
    private void WithLanguageCookie(string? lang)
    {
        var httpContext = new DefaultHttpContext();
        if (lang is not null)
            httpContext.Request.Headers.Cookie = $"{WebLanguage.CookieName}={lang}";

        _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    private async Task<List<ProjectViewModel>> InvokeAsync(string? lang = null)
    {
        WithLanguageCookie(lang);
        var result = await _sut.Index(null, CancellationToken.None) as ViewResult;
        return (List<ProjectViewModel>)result!.Model!;
    }

    [Fact]
    public async Task Index_ReturnsViewWithProjectList()
    {
        WithLanguageCookie(null);
        var result = await _sut.Index(null, CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        (await InvokeAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task Index_DefaultsToEnglish_WhenNoCookie()
    {
        var model = await InvokeAsync();

        model.Should().ContainSingle();
        model[0].NameEn.Should().Be("GitGlance");
        _api.Verify(a => a.GetProjectsAsync("en", It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _api.Verify(a => a.GetProjectsAsync("fa", It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Index_UsesFarsi_WhenLanguageCookieIsFa()
    {
        var model = await InvokeAsync("fa");

        model.Should().ContainSingle();
        // The single fetched language is mapped into the (legacy-named) NameEn slot.
        model[0].NameEn.Should().Be("گیت‌گلنس");
        _api.Verify(a => a.GetProjectsAsync("fa", It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Index_AssignsTechPillColorsFromFrontendMap()
    {
        var model = await InvokeAsync();

        var java = model[0].Techs.Single(t => t.Name == "Java");
        java.Color.Should().Be("#f0a63b");
    }
}
