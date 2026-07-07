using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;

namespace Portfolio.Web.Controllers;

public class WorkController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetWorkProjects();
        return View(model);
    }
}
