using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;

namespace Portfolio.Web.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetAboutViewModel();
        return View(model);
    }
}
