using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;

namespace Portfolio.Web.Controllers;

public class BlogController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetBlogPosts();
        return View(model);
    }
}
