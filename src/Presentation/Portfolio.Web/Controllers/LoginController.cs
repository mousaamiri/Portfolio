using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Web.Controllers;

// Public showcase login page (migration decision #4). UI-only: the form is a
// client-side simulation (login.js) — NO real authentication, NO POST action.
// This is separate from the admin panel's own login gate (Step 10).
public class LoginController : Controller
{
    public IActionResult Index() => View();
}
