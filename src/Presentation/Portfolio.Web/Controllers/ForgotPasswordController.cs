using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Web.Controllers;

// Public showcase forgot-password page (migration decision #4). UI-only:
// two-step card simulated entirely in forgot-password.js — no real reset,
// no POST action, no email is actually sent.
public class ForgotPasswordController : Controller
{
    public IActionResult Index() => View();
}
