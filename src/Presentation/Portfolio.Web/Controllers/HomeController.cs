using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;

namespace Portfolio.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetHomeViewModel();
        return View(model);
    }
}
