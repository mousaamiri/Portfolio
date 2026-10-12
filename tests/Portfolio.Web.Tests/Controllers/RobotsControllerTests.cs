using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Web.Controllers;
using Portfolio.Web.Features;

namespace Portfolio.Web.Tests.Controllers;

public class RobotsControllerTests
{
    private const string Host = "mousaamiri.ir";

    private static RobotsController Create(bool blogEnabled)
    {
        var controller = new RobotsController(Options.Create(
            new FeatureFlags { BlogEnabled = blogEnabled }));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString(Host);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    private static string Invoke(bool blogEnabled)
        => ((ContentResult)Create(blogEnabled).Index()).Content!;

    [Fact]
    public void Index_ReturnsPlainText()
    {
        var content = Invoke(blogEnabled: true);

        content.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Index_AlwaysAllowsRootAndDisallowsAdminAreas()
    {
        var content = Invoke(blogEnabled: true);

        content.Should().Contain("User-agent: *");
        content.Should().Contain("Allow: /");
        content.Should().Contain("Disallow: /Admin");
        content.Should().Contain("Disallow: /Login");
        content.Should().Contain("Disallow: /ForgotPassword");
    }

    [Fact]
    public void Index_ReferencesTheSitemap()
    {
        var content = Invoke(blogEnabled: true);

        content.Should().Contain($"Sitemap: https://{Host}/sitemap.xml");
    }

    [Fact]
    public void Index_WhenBlogEnabled_AllowsTheBlog()
    {
        var content = Invoke(blogEnabled: true);

        content.Should().NotContain("Disallow: /Blog");
    }

    [Fact]
    public void Index_WhenBlogDisabled_DisallowsTheBlog()
    {
        var content = Invoke(blogEnabled: false);

        content.Should().Contain("Disallow: /Blog");
    }
}
