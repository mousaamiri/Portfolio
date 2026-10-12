using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Portfolio.Web.Helpers;

/// <summary>
/// Renders <c>&lt;picture&gt;</c> source elements for modern image formats.
/// <c>@Html.ModernImageSources(url)</c> emits AVIF and WebP <c>&lt;source&gt;</c>
/// tags derived from a raster URL (extension swapped to <c>.avif</c>/<c>.webp</c>);
/// the caller keeps the original <c>&lt;img&gt;</c> as the format fallback:
///
/// <code>
/// &lt;picture&gt;
///     @@Html.ModernImageSources(Model.ImageUrl)
///     &lt;img src="@@Model.ImageUrl" ... /&gt;
/// &lt;/picture&gt;
/// </code>
///
/// Non-raster URLs (SVG), empty paths and anything without an extension emit
/// nothing, so a <c>&lt;picture&gt;</c> around them degrades to a plain
/// <c>&lt;img&gt;</c>. Attribute values are HTML-encoded by the tag builder.
/// </summary>
public static class PictureHtmlHelperExtensions
{
    private static readonly HashSet<string> RasterExtensions =
        new(StringComparer.OrdinalIgnoreCase) { "jpg", "jpeg", "png" };

    // The site ships one candidate per format at 2x the rendered size (see the
    // image-conversion step), so a plain srcset is enough — no width descriptors.
    private static readonly (string Format, string MimeType)[] ModernFormats =
        [ ("avif", "image/avif"), ("webp", "image/webp") ];

    public static IHtmlContent ModernImageSources(this IHtmlHelper html, string? src)
    {
        if (string.IsNullOrWhiteSpace(src))
            return HtmlString.Empty;

        var slash = src.LastIndexOf('/');
        var dot = src.LastIndexOf('.');
        if (dot <= slash)
            return HtmlString.Empty;

        // A query string (?w=…) or fragment after the extension is not part of it.
        var extensionSpan = src.AsSpan(dot + 1);
        var queryAt = extensionSpan.IndexOfAny(['?', '#']);
        var extension = (queryAt < 0 ? extensionSpan : extensionSpan[..queryAt]).ToString();

        if (!RasterExtensions.Contains(extension))
            return HtmlString.Empty;

        var basePath = src[..dot];

        var builder = new HtmlContentBuilder();
        foreach (var (format, mimeType) in ModernFormats)
        {
            var source = new TagBuilder("source");
            source.Attributes.Add("type", mimeType);
            source.Attributes.Add("srcset", $"{basePath}.{format}");
            builder.AppendHtml(source);
        }

        return builder;
    }
}
