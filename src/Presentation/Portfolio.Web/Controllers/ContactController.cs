using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class ContactController(IPortfolioApiClient api) : Controller
{
    // GET /Contact — contact info card (email/socials) now comes from the Profile
    // entity and the FAQ list from Portfolio.API. Falls back to the mock contact
    // info only when no profile is available. The form posts for real (Submit below).
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.Resolve(lang);
        var profile = await api.GetProfileAsync(language, cancellationToken);
        var info = MockDataService.GetContactViewModel();
        var faqs = await api.GetFaqsAsync(language, cancellationToken);

        var model = new ContactViewModel
        {
            Email = profile?.Email ?? info.Email,
            // Owner lists no phone; show nothing rather than the mock placeholder once a profile exists.
            Phone = profile is null ? info.Phone : string.Empty,
            GitHubUrl = profile?.GitHubUrl ?? info.GitHubUrl,
            LinkedInUrl = profile?.LinkedInUrl ?? info.LinkedInUrl,
            InstagramUrl = profile?.InstagramUrl ?? info.InstagramUrl,
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
