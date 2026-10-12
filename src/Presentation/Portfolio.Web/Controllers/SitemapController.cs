using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Web.Features;
using Portfolio.Web.Services;

namespace Portfolio.Web.Controllers;

/// <summary>
/// Serves the dynamic <c>/sitemap.xml</c>. The site is section-based with no
/// per-item detail pages, so the URL set is the fixed list of public pages —
/// no API call needed, which keeps the endpoint available even if the API is
/// down. Each page carries hreflang alternates for every supported language
/// (same URL + <c>?lang=</c>) plus an x-default.
/// </summary>
public class SitemapController(IOptions<FeatureFlags> features) : Controller
{
    // Public section pages (relative paths), in sitemap order. Non-public areas
    // (/Admin, /Login, /ForgotPassword, /Error) are intentionally excluded —
    // robots.txt disallows them. /Blog is appended only when its feature flag is
    // on (see Index below), so a disabled blog is de-indexed while its URL keeps
    // rendering — re-enabling is a one-line config flip.
    private static readonly string[] AlwaysVisiblePaths =
        ["/", "/Work", "/About", "/Experience", "/Contact"];

    [Route("sitemap.xml")]
    public IActionResult Index()
    {
        var paths = new List<string>(AlwaysVisiblePaths);
        if (features.Value.BlogEnabled)
            paths.Insert(4, "/Blog");

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        XNamespace xhtml = "http://www.w3.org/1999/xhtml";

        var urlset = new XElement(ns + "urlset",
            new XAttribute(XNamespace.Xmlns + "xhtml", xhtml));

        foreach (var path in paths)
        {
            var loc = baseUrl + path;
            var url = new XElement(ns + "url", new XElement(ns + "loc", loc));

            foreach (var lang in WebLanguage.Supported)
                url.Add(new XElement(xhtml + "link",
                    new XAttribute("rel", "alternate"),
                    new XAttribute("hreflang", lang),
                    new XAttribute("href", $"{loc}?lang={lang}")));

            url.Add(new XElement(xhtml + "link",
                new XAttribute("rel", "alternate"),
                new XAttribute("hreflang", "x-default"),
                new XAttribute("href", loc)));

            urlset.Add(url);
        }

        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null), urlset);
        return Content(doc.Declaration + Environment.NewLine + doc, "application/xml", Encoding.UTF8);
    }
}
