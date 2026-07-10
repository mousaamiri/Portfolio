using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class ContactController(IPortfolioApiClient api) : Controller
{
    // GET /Contact — email/socials prefer the Profile entity (fall back to the real
    // mock config); phone/location/website come from the mock (no Profile field yet).
    // FAQ list from Portfolio.API. The form posts for real (Submit below).
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.ResolveFromRequest(HttpContext, lang);
        var profile = await api.GetProfileAsync(language, cancellationToken);
        var info = MockDataService.GetContactViewModel();
        var faqs = await api.GetFaqsAsync(language, cancellationToken);

        var model = new ContactViewModel
        {
            Email = string.IsNullOrWhiteSpace(profile?.Email) ? info.Email : profile!.Email,
            Phone = info.Phone,
            Location = info.Location,
            Country = info.Country,
            CountryCode = info.CountryCode,
            GitHubUrl = string.IsNullOrWhiteSpace(profile?.GitHubUrl) ? info.GitHubUrl : profile!.GitHubUrl!,
            LinkedInUrl = string.IsNullOrWhiteSpace(profile?.LinkedInUrl) ? info.LinkedInUrl : profile!.LinkedInUrl!,
            InstagramUrl = string.IsNullOrWhiteSpace(profile?.InstagramUrl) ? info.InstagramUrl : profile!.InstagramUrl!,
            TelegramUrl = string.IsNullOrWhiteSpace(profile?.TelegramUrl) ? info.TelegramUrl : profile!.TelegramUrl!,
            TwitterUrl = string.IsNullOrWhiteSpace(profile?.TwitterUrl) ? info.TwitterUrl : profile!.TwitterUrl!,
            WebsiteUrl = string.IsNullOrWhiteSpace(profile?.WebsiteUrl) ? info.WebsiteUrl : profile!.WebsiteUrl!,
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
