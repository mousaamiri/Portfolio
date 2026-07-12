using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

/// <summary>
/// Global error endpoint wired via <c>app.UseExceptionHandler("/Error")</c>. When
/// Portfolio.API / the database is unreachable, <see cref="ApiUnavailableException"/>
/// bubbles up here and a clean, self-contained "can't reach the server" page is
/// shown (HTTP 503). Any other unhandled error shows a generic 500 variant. The
/// view depends on NO API call and NO shared layout, so it renders even in a full
/// outage.
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorController : Controller
{
    [Route("/Error")]
    public IActionResult Index()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var isServerDown = feature?.Error is ApiUnavailableException;

        Response.StatusCode = isServerDown
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status500InternalServerError;

        var model = new ErrorViewModel
        {
            Language = WebLanguage.ResolveFromRequest(HttpContext),
            IsServerUnavailable = isServerDown,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        return View("~/Views/Error.cshtml", model);
    }
}
