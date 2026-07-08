using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class ContactController(IPortfolioApiClient api) : Controller
{
    // GET /Contact — info card + FAQ still come from MockDataService (FAQ becomes
    // dynamic in E5). The form now posts for real (Submit below).
    public IActionResult Index()
    {
        var model = MockDataService.GetContactViewModel();
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
