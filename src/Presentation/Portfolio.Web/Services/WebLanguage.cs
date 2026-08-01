namespace Portfolio.Web.Services;

/// <summary>
/// Resolves the content language code used when calling the public API (<c>?lang</c>).
/// Server-side resolution; the language is persisted in the <see cref="CookieName"/>
/// cookie and applied on a full page reload (set via <c>LanguageController</c>).
/// Defaults to English.
/// </summary>
public static class WebLanguage
{
    public const string Default = "en";

    /// <summary>Cookie that persists the visitor's chosen language across requests.</summary>
    public const string CookieName = "portfolio-lang";

    public static readonly IReadOnlyCollection<string> Supported = ["en", "fa", "ar"];

    private static readonly HashSet<string> SupportedSet = new(Supported, StringComparer.OrdinalIgnoreCase);

    public static bool IsSupported(string? lang)
        => !string.IsNullOrWhiteSpace(lang) && SupportedSet.Contains(lang);

    public static string Resolve(string? lang)
        => IsSupported(lang) ? lang!.ToLowerInvariant() : Default;

    /// <summary>
    /// Resolves the language for the current request. Precedence: an explicit
    /// <c>?lang</c> query (deep-link/override) wins, then the persisted cookie,
    /// then the browser's <c>Accept-Language</c> header (first-visit detection),
    /// and finally the default. Query overrides do not mutate the cookie — only
    /// the language switch endpoint does that. Header detection only applies when
    /// no cookie is present, so a visitor's explicit choice is never overridden.
    /// </summary>
    public static string ResolveFromRequest(HttpContext httpContext, string? queryLang = null)
    {
        if (IsSupported(queryLang))
            return queryLang!.ToLowerInvariant();

        var cookie = httpContext?.Request?.Cookies[CookieName];
        if (IsSupported(cookie))
            return cookie!.ToLowerInvariant();

        var fromHeader = ResolveFromAcceptLanguage(httpContext?.Request);
        return fromHeader ?? Default;
    }

    /// <summary>
    /// Picks the first supported language from the request's <c>Accept-Language</c>
    /// header, comparing on the two-letter primary subtag (e.g. <c>fa-IR</c> → <c>fa</c>).
    /// Returns <c>null</c> when the header is absent or lists no supported language.
    /// </summary>
    private static string? ResolveFromAcceptLanguage(HttpRequest? request)
    {
        if (request is null)
            return null;

        var header = request.Headers.AcceptLanguage.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(header))
            return null;

        // Header shape: "fa-IR,fa;q=0.9,en;q=0.8". Walk entries in listed order and
        // return the first whose primary subtag we support.
        foreach (var part in header.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var tag = part.Split(';', 2)[0].Trim();
            if (tag.Length < 2)
                continue;

            var primary = tag[..2].ToLowerInvariant();
            if (SupportedSet.Contains(primary))
                return primary;
        }

        return null;
    }
}
