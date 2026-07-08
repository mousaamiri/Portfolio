using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class ContactController(IPortfolioApiClient api) : Controller
{
    // GET /Contact — FAQ list now comes from Portfolio.API; the contact info card
    // (email/phone/socials) stays mocked pending Profile coverage. The form posts
    // for real (Submit below).
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.Resolve(lang);
        var info = MockDataService.GetContactViewModel();
        var faqs = await api.GetFaqsAsync(language, cancellationToken);

        var model = new ContactViewModel
        {
            Email = info.Email,
            Phone = info.Phone,
            GitHubUrl = info.GitHubUrl,
            LinkedInUrl = info.LinkedInUrl,
            InstagramUrl = info.InstagramUrl,
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
