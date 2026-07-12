using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class ContactController(IPortfolioApiClient api) : Controller
{
    // GET /Contact — all contact details come from the Profile entity (Portfolio.API,
    // database-backed). FAQ list from Portfolio.API. The form posts for real (Submit below).
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.ResolveFromRequest(HttpContext, lang);
        var profile = await api.GetProfileAsync(language, cancellationToken);
        var faqs = await api.GetFaqsAsync(language, cancellationToken);

        var model = new ContactViewModel
        {
            Email = profile?.Email ?? string.Empty,
            Phone = profile?.Phone ?? string.Empty,
            Location = profile?.Location ?? string.Empty,
            Country = profile?.Country ?? string.Empty,
            CountryCode = profile?.CountryCode ?? string.Empty,
            GitHubUrl = profile?.GitHubUrl ?? string.Empty,
            LinkedInUrl = profile?.LinkedInUrl ?? string.Empty,
            InstagramUrl = profile?.InstagramUrl ?? string.Empty,
            TelegramUrl = profile?.TelegramUrl ?? string.Empty,
            TwitterUrl = profile?.TwitterUrl ?? string.Empty,
            WebsiteUrl = profile?.WebsiteUrl ?? string.Empty,
            Faqs = faqs.Select(ApiViewModelMapper.ToViewModel).ToList()
        };

        return View(model);
    }

    // POST /Contact/Submit — real contact-form submission, forwarded to the public
    // API which persists it as a Message (admin inbox). Replaces the old
    // client-side setTimeout simulation. Returns JSON for contact.js.
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] ContactFormModel form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false });

        var ok = await api.SendMessageAsync(new ContactMessageRequest
        {
            Name = form.Name,
            Email = form.Email,
            Subject = form.Subject,
            Body = form.Message,
            Interest = form.Interest
        }, cancellationToken);

        return Json(new { success = ok });
    }
}
