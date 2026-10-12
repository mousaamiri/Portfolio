namespace Portfolio.Web.Features;

/// <summary>
/// Toggles optional public-facing sections of the site. Bound from the
/// <c>Features</c> configuration section. Sections disabled here stay fully
/// functional (their controllers/views are untouched) but are unlinked from
/// every navigation surface and excluded from the SEO surfaces (sitemap.xml +
/// robots.txt), so search engines and visitors can no longer reach them.
/// </summary>
public sealed class FeatureFlags
{
    /// <summary>
    /// The public Blog page. When <c>false</c>, the blog is hidden from the
    /// navbar (desktop + mobile), command palette, footer, sitemap.xml and
    /// robots.txt. <see cref="BlogController"/> keeps serving <c>/Blog</c> so
    /// the page still renders for anyone with the URL and can be re-linked by
    /// flipping this flag alone — no code change or redeploy of content needed.
    /// </summary>
    public bool BlogEnabled { get; set; } = true;
}
