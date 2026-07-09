using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models;
using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

// Admin panel (Phase 3). Real, cookie-based authentication backed by the API's
// JWT auth: the login form posts server-side to Portfolio.API, and on success the
// returned JWT is stored inside the encrypted auth cookie (never exposed to the
// browser). Every panel action requires an authenticated cookie via [Authorize];
// the old client-side admin/admin sessionStorage gate has been removed.
[Authorize]
public class AdminController(IAdminApiClient adminApi, IAdminCrudClient crud) : Controller
{
    public IActionResult Index() => View();          // dashboard

    // ── Projects (real CRUD via Portfolio.API) ──
    public async Task<IActionResult> Projects(CancellationToken cancellationToken)
    {
        var projects = await adminApi.GetProjectsAsync("en", cancellationToken);
        var model = projects.Select(AdminProjectMapper.ToListItem).ToList();
        return View(model);
    }

    [HttpGet]
    public IActionResult ProjectCreate() => View("ProjectForm", new ProjectFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProjectCreate(ProjectFormModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View("ProjectForm", model);

        var id = await adminApi.CreateProjectAsync(AdminProjectMapper.ToCreateRequest(model), cancellationToken);
        if (id is null)
        {
            ModelState.AddModelError(string.Empty, "Could not create the project. The slug may already be in use.");
            return View("ProjectForm", model);
        }

        TempData["AdminMessage"] = "Project created.";
        return RedirectToAction(nameof(Projects));
    }

    [HttpGet]
    public async Task<IActionResult> ProjectEdit(Guid id, CancellationToken cancellationToken)
    {
        var (en, fa) = await adminApi.GetProjectBothLanguagesAsync(id, cancellationToken);
        if (en is null)
            return NotFound();

        return View("ProjectForm", AdminProjectMapper.ToFormModel(en, fa));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProjectEdit(Guid id, ProjectFormModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View("ProjectForm", model);

        var ok = await adminApi.UpdateProjectAsync(id, AdminProjectMapper.ToUpdateRequest(model), cancellationToken);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "Could not update the project.");
            return View("ProjectForm", model);
        }

        TempData["AdminMessage"] = "Project updated.";
        return RedirectToAction(nameof(Projects));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProjectDelete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await adminApi.DeleteProjectAsync(id, cancellationToken);
        TempData["AdminMessage"] = ok ? "Project deleted." : "Could not delete the project.";
        return RedirectToAction(nameof(Projects));
    }

    public IActionResult Articles() => View();
    public IActionResult Experiences() => View();
    public IActionResult Education() => View();

    // ── Skills (real CRUD) ──
    public async Task<IActionResult> Skills(CancellationToken ct)
    {
        var skills = await crud.ListAsync<SkillApiDto>("skills", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Skills",
            Subtitle = "Manage your technical skills",
            SidebarKey = "skills",
            CreateAction = nameof(SkillCreate),
            EditAction = nameof(SkillEdit),
            DeleteAction = nameof(SkillDelete),
            CreateLabel = "New Skill",
            Headers = ["Name", "Category", "Proficiency"],
            ShowStatusColumn = true,
            Rows = skills.Select(AdminSkillMapper.ToRow).ToList()
        });
    }

    [HttpGet]
    public IActionResult SkillCreate() => View("SkillForm", new SkillFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SkillCreate(SkillFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("SkillForm", model);
        var id = await crud.CreateAsync("skills", AdminSkillMapper.ToRequest(model), ct);
        if (id is null)
        {
            ModelState.AddModelError(string.Empty, "Could not create the skill.");
            return View("SkillForm", model);
        }
        TempData["AdminMessage"] = "Skill created.";
        return RedirectToAction(nameof(Skills));
    }

    [HttpGet]
    public async Task<IActionResult> SkillEdit(Guid id, CancellationToken ct)
    {
        var en = await crud.GetAsync<SkillApiDto>("skills", id, "en", ct);
        if (en is null) return NotFound();
        var fa = await crud.GetAsync<SkillApiDto>("skills", id, "fa", ct);
        return View("SkillForm", AdminSkillMapper.ToFormModel(en, fa));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SkillEdit(Guid id, SkillFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("SkillForm", model);
        var ok = await crud.UpdateAsync("skills", id, AdminSkillMapper.ToRequest(model), ct);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "Could not update the skill.");
            return View("SkillForm", model);
        }
        TempData["AdminMessage"] = "Skill updated.";
        return RedirectToAction(nameof(Skills));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SkillDelete(Guid id, CancellationToken ct)
    {
        var ok = await crud.DeleteAsync("skills", id, ct);
        TempData["AdminMessage"] = ok ? "Skill deleted." : "Could not delete the skill.";
        return RedirectToAction(nameof(Skills));
    }

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
