using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Web.Controllers;

// Admin panel (migration decision / addendum). This is a CLIENT-SIDE app:
// data.js (in-memory AdminData store) + shared.js (sidebar + generic AdminCRUD)
// do all the work. Auth is an INSECURE DEMO GATE (sessionStorage["admin_auth"],
// admin/admin checked in plain JS) — ported exactly as-is, NOT real security,
// and NO server-side authorization is applied here in this phase. CRUD is
// in-memory / non-persistent, matching the static admin panel.
//
// Routes (conventional {controller}/{action}): /Admin (dashboard),
// /Admin/Projects, /Admin/Articles, /Admin/Experiences, /Admin/Education,
// /Admin/Skills, /Admin/Testimonials, /Admin/Messages, /Admin/Login,
// /Admin/ForgotPassword.
public class AdminController : Controller
{
    public IActionResult Index() => View();          // dashboard

    public IActionResult Projects() => View();
    public IActionResult Articles() => View();
    public IActionResult Experiences() => View();
    public IActionResult Education() => View();
    public IActionResult Skills() => View();
    public IActionResult Testimonials() => View();
    public IActionResult Messages() => View();

    // Insecure demo gate (client-side only — see class remarks).
    public IActionResult Login() => View();
    public IActionResult ForgotPassword() => View();
}
