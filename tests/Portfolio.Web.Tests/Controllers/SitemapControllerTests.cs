using System.Xml.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Web.Controllers;
using Portfolio.Web.Features;

namespace Portfolio.Web.Tests.Controllers;

public class SitemapControllerTests
{
    private const string Host = "mousaamiri.ir";

    private static readonly string[] VisiblePaths =
        ["/", "/Work", "/About", "/Experience", "/Contact"];

    private readonly SitemapController _sut;

    public SitemapControllerTests()
    {
        _sut = Create(featuresEnabled: true);
    }

    private static SitemapController Create(bool featuresEnabled)
    {
        var controller = new SitemapController(Options.Create(
            new FeatureFlags { BlogEnabled = featuresEnabled }));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString(Host);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    private static List<string> Locs(ContentResult result)
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        return XDocument.Parse(result.Content!)
            .Descendants(ns + "loc")
            .Select(e => e.Value)
            .ToList();
    }

    private ContentResult Invoke() => (ContentResult)_sut.Index();

    [Fact]
    public void Index_ReturnsXmlContent()
    {
        var result = Invoke();

        result.ContentType.Should().Be("application/xml; charset=utf-8");
        result.Content.Should().Contain("<?xml");
        result.Content.Should().Contain("http://www.sitemaps.org/schemas/sitemap/0.9");
    }

    [Fact]
    public void Index_WhenBlogEnabled_IncludesBlogAndEveryPublicPage()
    {
        var content = Create(featuresEnabled: true).Index() as ContentResult;

        var locs = Locs(content!);

        var expected = new List<string> { "/", "/Work", "/About", "/Experience", "/Blog", "/Contact" };
        locs.Should().HaveCount(expected.Count);
        foreach (var path in expected)
            locs.Should().Contain($"https://{Host}{path}");
    }

    [Fact]
    public void Index_WhenBlogDisabled_ExcludesBlogButKeepsEveryOtherPage()
    {
        var content = Create(featuresEnabled: false).Index() as ContentResult;

        var locs = Locs(content!);

        locs.Should().HaveCount(VisiblePaths.Length);
        foreach (var path in VisiblePaths)
            locs.Should().Contain($"https://{Host}{path}");
        locs.Should().NotContain($"https://{Host}/Blog");
    }

    [Fact]
    public void Index_EmitsHreflangAlternatesForEachLanguagePlusXDefault()
    {
        var content = Invoke().Content!;
        XNamespace xhtml = "http://www.w3.org/1999/xhtml";

        var hreflangs = XDocument.Parse(content)
            .Descendants(xhtml + "link")
            .Select(e => e.Attribute("hreflang")!.Value)
            .ToList();

        // Each of the 6 pages emits en/fa/ar + x-default.
        hreflangs.Should().Contain("en").And.Contain("fa").And.Contain("ar").And.Contain("x-default");
        hreflangs.Count(h => h == "en").Should().Be(6);
        hreflangs.Count(h => h == "x-default").Should().Be(6);
    }

    [Fact]
    public void Index_HreflangAlternatesCarryLangQuery()
    {
        var content = Invoke().Content!;
        XNamespace xhtml = "http://www.w3.org/1999/xhtml";

        var faHref = XDocument.Parse(content)
            .Descendants(xhtml + "link")
            .First(e => e.Attribute("hreflang")!.Value == "fa")
            .Attribute("href")!.Value;

        faHref.Should().Be($"https://{Host}/?lang=fa");
    }
}
