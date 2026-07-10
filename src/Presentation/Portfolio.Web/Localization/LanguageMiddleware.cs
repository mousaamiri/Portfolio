using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Localization;

/// <summary>
/// Resolves the request language from the <c>portfolio-lang</c> cookie (or a
/// <c>?lang</c> override) and loads that language's UI-chrome map from the API
/// (DB) into the scoped <see cref="LocalizationState"/>. English is DB-backed too
/// (so it's editable from the admin panel); the views' inline <c>@Html.T</c>
/// defaults remain a last-resort fallback. Admin/auth areas are skipped — they
/// render English/LTR from their own literals only.
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
        state.Map = await api.GetUiTranslationsAsync(language, context.RequestAborted);

        await next(context);
    }
}
