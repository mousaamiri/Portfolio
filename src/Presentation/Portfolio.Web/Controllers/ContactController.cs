using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;

namespace Portfolio.Web.Controllers;

public class ContactController : Controller
{
    // GET /Contact — the form itself is a client-side simulation (contact.js);
    // there is deliberately no [HttpPost] action in this phase (no backend).
    public IActionResult Index()
    {
        var model = MockDataService.GetContactViewModel();
        return View(model);
    }
}
