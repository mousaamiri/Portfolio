using System.Xml.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Controllers;

namespace Portfolio.Web.Tests.Controllers;

public class SitemapControllerTests
{
    private const string Host = "mousaamiri.ir";
    private static readonly string[] ExpectedPaths =
        ["/", "/Work", "/About", "/Experience", "/Blog", "/Contact"];

    private readonly SitemapController _sut = new();

    public SitemapControllerTests()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString(Host);
        _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };
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
    public void Index_ContainsLocForEachPublicPage()
    {
        var content = Invoke().Content!;
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var locs = XDocument.Parse(content)
            .Descendants(ns + "loc")
            .Select(e => e.Value)
            .ToList();

        locs.Should().HaveCount(ExpectedPaths.Length);
        foreach (var path in ExpectedPaths)
            locs.Should().Contain($"https://{Host}{path}");
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
        hreflangs.Count(h => h == "en").Should().Be(ExpectedPaths.Length);
        hreflangs.Count(h => h == "x-default").Should().Be(ExpectedPaths.Length);
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
