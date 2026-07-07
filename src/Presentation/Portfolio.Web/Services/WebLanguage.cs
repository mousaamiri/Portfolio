namespace Portfolio.Web.Services;

/// <summary>
/// Resolves the content language code used when calling the public API (<c>?lang</c>).
/// Server-side resolution per the Phase 2 decision; defaults to English.
/// </summary>
public static class WebLanguage
{
    public const string Default = "en";

    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
    {
        "en", "fa", "ar"
    };

    public static string Resolve(string? lang)
    {
        if (!string.IsNullOrWhiteSpace(lang) && Supported.Contains(lang))
            return lang.ToLowerInvariant();

        return Default;
    }
}
