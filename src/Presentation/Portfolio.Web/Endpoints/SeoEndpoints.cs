using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using Portfolio.Web.Features;

namespace Portfolio.Web.Endpoints;

/// <summary>
/// Utility endpoints served by the SPA host: a health probe plus the dynamic
/// <c>/robots.txt</c> and <c>/sitemap.xml</c>. They live server-side so they stay
/// in sync with the <c>Features</c> flags: a disabled section is disallowed in
/// robots.txt and dropped from the sitemap in one move, with no static file to
/// forget.
/// </summary>
public static class SeoEndpoints
{
    // Public section pages (relative paths), in sitemap order. Non-public areas
    // (/Admin, /Login, /ForgotPassword) are intentionally excluded.
    private static readonly string[] AlwaysVisiblePaths =
        ["/", "/Work", "/About", "/Experience", "/Contact"];

    private static readonly string[] SupportedLanguages = ["en", "fa", "ar"];

    public static IEndpointRouteBuilder MapSeoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health", () => Results.Text("ok", "text/plain"));

        endpoints.MapGet("/robots.txt", (HttpContext context, IOptions<FeatureFlags> features) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Allow: /");

            // Non-public areas are never crawlable.
            sb.AppendLine("Disallow: /Admin");
            sb.AppendLine("Disallow: /Login");
            sb.AppendLine("Disallow: /ForgotPassword");

            // A disabled blog is de-indexed; re-enabling only needs the flag flip.
            if (!features.Value.BlogEnabled)
                sb.AppendLine("Disallow: /Blog");

            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            sb.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");

            return Results.Text(sb.ToString(), "text/plain", Encoding.UTF8);
        });

        endpoints.MapGet("/sitemap.xml", (HttpContext context, IOptions<FeatureFlags> features) =>
        {
            var paths = new List<string>(AlwaysVisiblePaths);
            if (features.Value.BlogEnabled)
                paths.Insert(4, "/Blog");

            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace xhtml = "http://www.w3.org/1999/xhtml";

            var urlset = new XElement(ns + "urlset",
                new XAttribute(XNamespace.Xmlns + "xhtml", xhtml));

            foreach (var path in paths)
            {
                var loc = baseUrl + path;
                var url = new XElement(ns + "url", new XElement(ns + "loc", loc));

                foreach (var lang in SupportedLanguages)
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
            return Results.Text(doc.Declaration + Environment.NewLine + doc, "application/xml", Encoding.UTF8);
        });

        return endpoints;
    }
}
