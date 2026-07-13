using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;

namespace Portfolio.Web.Controllers;

/// <summary>
/// Serves the dynamic <c>/sitemap.xml</c>. The site is section-based with no
/// per-item detail pages, so the URL set is the fixed list of public pages —
/// no API call needed, which keeps the endpoint available even if the API is
/// down. Each page carries hreflang alternates for every supported language
/// (same URL + <c>?lang=</c>) plus an x-default.
/// </summary>
public class SitemapController : Controller
{
    // Public section pages (relative paths). Non-public areas (/Admin, /Login,
    // /ForgotPassword, /Error) are intentionally excluded — robots.txt disallows them.
    private static readonly string[] Paths =
        ["/", "/Work", "/About", "/Experience", "/Blog", "/Contact"];

    [Route("sitemap.xml")]
    public IActionResult Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        XNamespace xhtml = "http://www.w3.org/1999/xhtml";

        var urlset = new XElement(ns + "urlset",
            new XAttribute(XNamespace.Xmlns + "xhtml", xhtml));

        foreach (var path in Paths)
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
