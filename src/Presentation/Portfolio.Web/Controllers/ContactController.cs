using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    // API which persists it as a Message (admin inbox) and emails a notification.
    // Protected by a honeypot (Website decoy) and per-IP rate limiting ("contact").
    // Returns JSON for contact.js.
    [HttpPost]
    [EnableRateLimiting("contact")]
    public async Task<IActionResult> Submit([FromBody] ContactFormModel form, CancellationToken cancellationToken)
    {
        // Honeypot: a real visitor never sees or fills the Website field. A bot that
        // does gets a fake success — nothing is persisted or emailed.
        if (!string.IsNullOrWhiteSpace(form.Website))
            return Json(new { success = true });

        if (!ModelState.IsValid)
            return BadRequest(new { success = false });

        var ok = await api.SendMessageAsync(new ContactMessageRequest
        {
            Name = form.Name,
            Email = form.Email,
            Phone = form.Phone,
            Subject = form.Subject,
            Body = form.Message,
            Interest = form.Interest
        }, cancellationToken);

        return Json(new { success = ok });
    }
}
