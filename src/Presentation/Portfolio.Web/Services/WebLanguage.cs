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
    /// Resolves the language for the current request: an explicit <c>?lang</c>
    /// query wins (deep-link/override), otherwise the persisted cookie, else the
    /// default. Query overrides do not mutate the cookie — only the language
    /// switch endpoint does that.
    /// </summary>
    public static string ResolveFromRequest(HttpContext httpContext, string? queryLang = null)
    {
        if (IsSupported(queryLang))
            return queryLang!.ToLowerInvariant();

        var cookie = httpContext?.Request?.Cookies[CookieName];
        return Resolve(cookie);
    }
}
