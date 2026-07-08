namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// One blog article card (metadata). Per CONTENT_CLASSIFICATION.md the public
/// BlogPostViewModel is title/date/summary/category/read-time — the full HTML
/// article BODIES stay in blog.js (finished English-only copy, no i18n; they
/// only appear in the JS-built reading modal). The Blog page is English-only in
/// the static site (no parallel FA array, no data-i18n), so this is single-language.
/// </summary>
public class BlogPostViewModel
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Excerpt { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public int ReadTime { get; init; }

    /// <summary>Full HTML article body, shown in the client-side reading modal.</summary>
    public string Body { get; init; } = string.Empty;

    /// <summary>Card date in blog.js's format, e.g. "TUE MAY 26 2026".</summary>
    public string DisplayDate =>
        Date.ToString("ddd MMM dd yyyy", System.Globalization.CultureInfo.InvariantCulture).ToUpperInvariant();
}
