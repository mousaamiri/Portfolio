using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Portfolio.Web.Localization;

/// <summary>
/// Razor localization helpers. <c>@Html.T(key, english)</c> renders localized
/// UI chrome: the DB value for the current (non-English) language, falling back
/// to the inline <paramref name="english"/> default otherwise. Values may contain
/// trusted markup (e.g. bio <c>&lt;span class="highlight"&gt;</c>), so element
/// content is emitted raw — matching the retired client-side <c>innerHTML</c>
/// behavior. Use <see cref="TA"/> for attribute contexts (placeholder/title/aria)
/// where a plain, HTML-encoded string is required.
/// </summary>
public static class LocalizationHtmlHelperExtensions
{
    public static IHtmlContent T(this IHtmlHelper html, string key, string english)
        => new HtmlString(Resolve(html, key, english));

    public static string TA(this IHtmlHelper html, string key, string english)
        => Resolve(html, key, english);

    private static string Resolve(IHtmlHelper html, string key, string english)
    {
        var state = (LocalizationState?)html.ViewContext.HttpContext.RequestServices
            .GetService(typeof(LocalizationState));

        if (state is not null && state.Language != Services.WebLanguage.Default
            && state.Map.TryGetValue(key, out var value)
            && !string.IsNullOrEmpty(value))
        {
            return value;
        }

        return english;
    }
}
