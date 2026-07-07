using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;

namespace Portfolio.Web.Controllers;

public class ExperienceController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetExperiencePageViewModel();
        return View(model);
    }
}
