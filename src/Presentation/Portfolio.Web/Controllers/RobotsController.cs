using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Web.Features;

namespace Portfolio.Web.Controllers;

/// <summary>
/// Serves the dynamic <c>/robots.txt</c>. Kept server-side (rather than a static
/// wwwroot file) so it stays in sync with <see cref="SitemapController"/> and the
/// <c>Features</c> flags: a disabled section is disallowed here and dropped from
/// the sitemap in one move, with no static file to forget. The sitemap reference
/// uses the public host so crawlers always find the current URL set.
/// </summary>
public class RobotsController(IOptions<FeatureFlags> features) : Controller
{
    [Route("robots.txt")]
    public IActionResult Index()
    {
        var sb = new StringBuilder();
        sb.AppendLine("User-agent: *");
        sb.AppendLine("Allow: /");

        // Non-public areas are never crawlable. The admin panel, login and
        // password-recovery pages hold no content worth indexing and exposing
        // them in a sitemap would just invite probing.
        sb.AppendLine("Disallow: /Admin");
        sb.AppendLine("Disallow: /Login");
        sb.AppendLine("Disallow: /ForgotPassword");

        // A disabled blog is de-indexed: no sitemap entry + explicit disallow so
        // already-indexed URLs get dropped. The page still renders for anyone with
        // the URL, so re-enabling only needs flipping Features:BlogEnabled.
        if (!features.Value.BlogEnabled)
            sb.AppendLine("Disallow: /Blog");

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        sb.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");

        return Content(sb.ToString(), "text/plain", Encoding.UTF8);
    }
}
