using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;

namespace Portfolio.Web.Controllers;

/// <summary>
/// Persists the visitor's chosen UI language in the <c>portfolio-lang</c> cookie
/// and reloads the page server-side, so both dynamic content (from the API/DB)
/// and UI chrome (from the ui-translations map) render in that language.
/// </summary>
public class LanguageController : Controller
{
    [HttpGet("/set-language")]
    public IActionResult Set(string lang, string? returnUrl)
    {
        var resolved = WebLanguage.Resolve(lang);

        Response.Cookies.Append(WebLanguage.CookieName, resolved, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            HttpOnly = false, // readable by the client so JS chrome can pick up the language without a round-trip
            Secure = Request.IsHttps
        });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}
