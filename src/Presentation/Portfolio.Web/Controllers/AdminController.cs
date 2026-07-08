using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

// Admin panel (Phase 3). Real, cookie-based authentication backed by the API's
// JWT auth: the login form posts server-side to Portfolio.API, and on success the
// returned JWT is stored inside the encrypted auth cookie (never exposed to the
// browser). Every panel action requires an authenticated cookie via [Authorize];
// the old client-side admin/admin sessionStorage gate has been removed.
[Authorize]
public class AdminController(IAdminApiClient adminApi) : Controller
{
    public IActionResult Index() => View();          // dashboard

    public IActionResult Projects() => View();
    public IActionResult Articles() => View();
    public IActionResult Experiences() => View();
    public IActionResult Education() => View();
    public IActionResult Skills() => View();
    public IActionResult Testimonials() => View();
    public IActionResult Messages() => View();

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Index));

        ViewData["ReturnUrl"] = returnUrl;
        return View(new AdminLoginModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginModel model, string? returnUrl, CancellationToken cancellationToken)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await adminApi.LoginAsync(model.Username, model.Password, cancellationToken);
        if (result is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, model.Username),
            new("access_token", result.Token)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = result.ExpiresAt
            });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult ForgotPassword() => View();
}
