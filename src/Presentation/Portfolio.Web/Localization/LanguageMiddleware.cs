using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Localization;

/// <summary>
/// Resolves the request language from the <c>portfolio-lang</c> cookie (or a
/// <c>?lang</c> override) and, for non-English, loads the UI-chrome map from the
/// API (DB) into the scoped <see cref="LocalizationState"/>. Admin/auth areas are
/// skipped — they render English/LTR only.
/// </summary>
public class LanguageMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, LocalizationState state, IPortfolioApiClient api)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        var queryLang = context.Request.Query["lang"].ToString();
        var language = WebLanguage.ResolveFromRequest(context, queryLang);
        state.Language = language;

        if (language != WebLanguage.Default)
            state.Map = await api.GetUiTranslationsAsync(language, context.RequestAborted);

        await next(context);
    }
}
