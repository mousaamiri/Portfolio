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
public class AdminController(IAdminApiClient adminApi, IAdminCrudClient crud, IPortfolioApiClient publicApi) : Controller
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

    // ── Faq (real CRUD) ──
    public async Task<IActionResult> Faqs(CancellationToken ct)
    {
        var faqs = await crud.ListAsync<FaqApiDto>("faqs", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "FAQ", Subtitle = "Manage contact-page questions", SidebarKey = "faqs",
            CreateAction = nameof(FaqCreate), EditAction = nameof(FaqEdit), DeleteAction = nameof(FaqDelete),
            CreateLabel = "New Question", Headers = ["Question", "Answer"], ShowStatusColumn = true,
            Rows = faqs.Select(AdminFaqMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult FaqCreate() => View("FaqForm", new FaqFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> FaqCreate(FaqFormModel m, CancellationToken ct)
        => CreateEntity("faqs", m, "FaqForm", nameof(Faqs), AdminFaqMapper.ToRequest, "FAQ", ct);
    [HttpGet] public Task<IActionResult> FaqEdit(Guid id, CancellationToken ct)
        => EditForm<FaqApiDto, FaqFormModel>("faqs", id, "FaqForm", AdminFaqMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> FaqEdit(Guid id, FaqFormModel m, CancellationToken ct)
        => UpdateEntity("faqs", id, m, "FaqForm", nameof(Faqs), AdminFaqMapper.ToRequest, "FAQ", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> FaqDelete(Guid id, CancellationToken ct)
        => DeleteEntity("faqs", id, nameof(Faqs), "FAQ", ct);

    // ── Interest (real CRUD) ──
    public async Task<IActionResult> Interests(CancellationToken ct)
    {
        var items = await crud.ListAsync<InterestApiDto>("interests", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Interests", Subtitle = "Manage About-page interests", SidebarKey = "interests",
            CreateAction = nameof(InterestCreate), EditAction = nameof(InterestEdit), DeleteAction = nameof(InterestDelete),
            CreateLabel = "New Interest", Headers = ["Label", "Icon"], ShowStatusColumn = true,
            Rows = items.Select(AdminInterestMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult InterestCreate() => View("InterestForm", new InterestFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> InterestCreate(InterestFormModel m, CancellationToken ct)
        => CreateEntity("interests", m, "InterestForm", nameof(Interests), AdminInterestMapper.ToRequest, "Interest", ct);
    [HttpGet] public Task<IActionResult> InterestEdit(Guid id, CancellationToken ct)
        => EditForm<InterestApiDto, InterestFormModel>("interests", id, "InterestForm", AdminInterestMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> InterestEdit(Guid id, InterestFormModel m, CancellationToken ct)
        => UpdateEntity("interests", id, m, "InterestForm", nameof(Interests), AdminInterestMapper.ToRequest, "Interest", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> InterestDelete(Guid id, CancellationToken ct)
        => DeleteEntity("interests", id, nameof(Interests), "Interest", ct);

    // ── TimelineEntry (real CRUD) ──
    public async Task<IActionResult> Timeline(CancellationToken ct)
    {
        var items = await crud.ListAsync<TimelineEntryApiDto>("timeline", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Timeline", Subtitle = "Manage About-page journey", SidebarKey = "timeline",
            CreateAction = nameof(TimelineCreate), EditAction = nameof(TimelineEdit), DeleteAction = nameof(TimelineDelete),
            CreateLabel = "New Entry", Headers = ["Year", "Title", "Icon"], ShowStatusColumn = true,
            Rows = items.Select(AdminTimelineMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult TimelineCreate() => View("TimelineForm", new TimelineFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> TimelineCreate(TimelineFormModel m, CancellationToken ct)
        => CreateEntity("timeline", m, "TimelineForm", nameof(Timeline), AdminTimelineMapper.ToRequest, "Timeline entry", ct);
    [HttpGet] public Task<IActionResult> TimelineEdit(Guid id, CancellationToken ct)
        => EditForm<TimelineEntryApiDto, TimelineFormModel>("timeline", id, "TimelineForm", AdminTimelineMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> TimelineEdit(Guid id, TimelineFormModel m, CancellationToken ct)
        => UpdateEntity("timeline", id, m, "TimelineForm", nameof(Timeline), AdminTimelineMapper.ToRequest, "Timeline entry", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> TimelineDelete(Guid id, CancellationToken ct)
        => DeleteEntity("timeline", id, nameof(Timeline), "Timeline entry", ct);

    // ── StatCounter (real CRUD) ──
    public async Task<IActionResult> Stats(CancellationToken ct)
    {
        var items = await crud.ListAsync<StatCounterApiDto>("stats", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Stats", Subtitle = "Manage About-page footprint counters", SidebarKey = "stats",
            CreateAction = nameof(StatCreate), EditAction = nameof(StatEdit), DeleteAction = nameof(StatDelete),
            CreateLabel = "New Stat", Headers = ["Label", "Target", "Icon"], ShowStatusColumn = true,
            Rows = items.Select(AdminStatMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult StatCreate() => View("StatForm", new StatFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> StatCreate(StatFormModel m, CancellationToken ct)
        => CreateEntity("stats", m, "StatForm", nameof(Stats), AdminStatMapper.ToRequest, "Stat", ct);
    [HttpGet] public Task<IActionResult> StatEdit(Guid id, CancellationToken ct)
        => EditForm<StatCounterApiDto, StatFormModel>("stats", id, "StatForm", AdminStatMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> StatEdit(Guid id, StatFormModel m, CancellationToken ct)
        => UpdateEntity("stats", id, m, "StatForm", nameof(Stats), AdminStatMapper.ToRequest, "Stat", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> StatDelete(Guid id, CancellationToken ct)
        => DeleteEntity("stats", id, nameof(Stats), "Stat", ct);

    // ── Generic CRUD helpers (shared by the entities above) ──
    private async Task<IActionResult> CreateEntity<TForm, TRequest>(
        string resource, TForm model, string formView, string listAction,
        Func<TForm, TRequest> toRequest, string label, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(formView, model);
        var id = await crud.CreateAsync(resource, toRequest(model), ct);
        if (id is null)
        {
            ModelState.AddModelError(string.Empty, $"Could not create the {label.ToLowerInvariant()}.");
            return View(formView, model);
        }
        TempData["AdminMessage"] = $"{label} created.";
        return RedirectToAction(listAction);
    }

    private async Task<IActionResult> UpdateEntity<TForm, TRequest>(
        string resource, Guid id, TForm model, string formView, string listAction,
        Func<TForm, TRequest> toRequest, string label, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(formView, model);
        var ok = await crud.UpdateAsync(resource, id, toRequest(model), ct);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, $"Could not update the {label.ToLowerInvariant()}.");
            return View(formView, model);
        }
        TempData["AdminMessage"] = $"{label} updated.";
        return RedirectToAction(listAction);
    }

    private async Task<IActionResult> EditForm<TDto, TForm>(
        string resource, Guid id, string formView,
        Func<TDto, TDto?, TForm> toForm, CancellationToken ct) where TDto : class
    {
        var en = await crud.GetAsync<TDto>(resource, id, "en", ct);
        if (en is null) return NotFound();
        var fa = await crud.GetAsync<TDto>(resource, id, "fa", ct);
        return View(formView, toForm(en, fa));
    }

    private async Task<IActionResult> DeleteEntity(string resource, Guid id, string listAction, string label, CancellationToken ct)
    {
        var ok = await crud.DeleteAsync(resource, id, ct);
        TempData["AdminMessage"] = ok ? $"{label} deleted." : $"Could not delete the {label.ToLowerInvariant()}.";
        return RedirectToAction(listAction);
    }

    // ── Article (real CRUD) ──
    public async Task<IActionResult> Articles(CancellationToken ct)
    {
        var items = await crud.ListAsync<ArticleApiDto>("articles", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Articles", Subtitle = "Manage blog articles", SidebarKey = "articles",
            CreateAction = nameof(ArticleCreate), EditAction = nameof(ArticleEdit), DeleteAction = nameof(ArticleDelete),
            CreateLabel = "New Article", Headers = ["Title", "Category", "Published"], ShowStatusColumn = true,
            Rows = items.Select(AdminArticleMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult ArticleCreate() => View("ArticleForm", new ArticleFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> ArticleCreate(ArticleFormModel m, CancellationToken ct)
        => CreateEntity("articles", m, "ArticleForm", nameof(Articles), AdminArticleMapper.ToRequest, "Article", ct);
    [HttpGet] public Task<IActionResult> ArticleEdit(Guid id, CancellationToken ct)
        => EditForm<ArticleApiDto, ArticleFormModel>("articles", id, "ArticleForm", AdminArticleMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> ArticleEdit(Guid id, ArticleFormModel m, CancellationToken ct)
        => UpdateEntity("articles", id, m, "ArticleForm", nameof(Articles), AdminArticleMapper.ToRequest, "Article", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> ArticleDelete(Guid id, CancellationToken ct)
        => DeleteEntity("articles", id, nameof(Articles), "Article", ct);

    // ── Experience (real CRUD) ──
    public async Task<IActionResult> Experiences(CancellationToken ct)
    {
        var items = await crud.ListAsync<ExperienceApiDto>("experiences", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Experience", Subtitle = "Manage professional history", SidebarKey = "experiences",
            CreateAction = nameof(ExperienceCreate), EditAction = nameof(ExperienceEdit), DeleteAction = nameof(ExperienceDelete),
            CreateLabel = "New Experience", Headers = ["Job title", "Company", "Dates"], ShowStatusColumn = true,
            Rows = items.Select(AdminExperienceMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult ExperienceCreate() => View("ExperienceForm", new ExperienceFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> ExperienceCreate(ExperienceFormModel m, CancellationToken ct)
        => CreateEntity("experiences", m, "ExperienceForm", nameof(Experiences), AdminExperienceMapper.ToRequest, "Experience", ct);
    [HttpGet] public Task<IActionResult> ExperienceEdit(Guid id, CancellationToken ct)
        => EditForm<ExperienceApiDto, ExperienceFormModel>("experiences", id, "ExperienceForm", AdminExperienceMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> ExperienceEdit(Guid id, ExperienceFormModel m, CancellationToken ct)
        => UpdateEntity("experiences", id, m, "ExperienceForm", nameof(Experiences), AdminExperienceMapper.ToRequest, "Experience", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> ExperienceDelete(Guid id, CancellationToken ct)
        => DeleteEntity("experiences", id, nameof(Experiences), "Experience", ct);

    // ── Education (real CRUD) ──
    public async Task<IActionResult> Education(CancellationToken ct)
    {
        var items = await crud.ListAsync<EducationApiDto>("educations", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Education", Subtitle = "Manage education history", SidebarKey = "education",
            CreateAction = nameof(EducationCreate), EditAction = nameof(EducationEdit), DeleteAction = nameof(EducationDelete),
            CreateLabel = "New Education", Headers = ["Degree", "Institution", "Dates"], ShowStatusColumn = true,
            Rows = items.Select(AdminEducationMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult EducationCreate() => View("EducationForm", new EducationFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> EducationCreate(EducationFormModel m, CancellationToken ct)
        => CreateEntity("educations", m, "EducationForm", nameof(Education), AdminEducationMapper.ToRequest, "Education", ct);
    [HttpGet] public Task<IActionResult> EducationEdit(Guid id, CancellationToken ct)
        => EditForm<EducationApiDto, EducationFormModel>("educations", id, "EducationForm", AdminEducationMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> EducationEdit(Guid id, EducationFormModel m, CancellationToken ct)
        => UpdateEntity("educations", id, m, "EducationForm", nameof(Education), AdminEducationMapper.ToRequest, "Education", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> EducationDelete(Guid id, CancellationToken ct)
        => DeleteEntity("educations", id, nameof(Education), "Education", ct);


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

    // ── Testimonial (real CRUD) ──
    public async Task<IActionResult> Testimonials(CancellationToken ct)
    {
        var items = await crud.ListAsync<TestimonialApiDto>("testimonials", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Testimonials", Subtitle = "Manage endorsements", SidebarKey = "testimonials",
            CreateAction = nameof(TestimonialCreate), EditAction = nameof(TestimonialEdit), DeleteAction = nameof(TestimonialDelete),
            CreateLabel = "New Testimonial", Headers = ["Name", "Role", "Quote"], ShowStatusColumn = true,
            Rows = items.Select(AdminTestimonialMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult TestimonialCreate() => View("TestimonialForm", new TestimonialFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> TestimonialCreate(TestimonialFormModel m, CancellationToken ct)
        => CreateEntity("testimonials", m, "TestimonialForm", nameof(Testimonials), AdminTestimonialMapper.ToRequest, "Testimonial", ct);
    [HttpGet] public Task<IActionResult> TestimonialEdit(Guid id, CancellationToken ct)
        => EditForm<TestimonialApiDto, TestimonialFormModel>("testimonials", id, "TestimonialForm", AdminTestimonialMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> TestimonialEdit(Guid id, TestimonialFormModel m, CancellationToken ct)
        => UpdateEntity("testimonials", id, m, "TestimonialForm", nameof(Testimonials), AdminTestimonialMapper.ToRequest, "Testimonial", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> TestimonialDelete(Guid id, CancellationToken ct)
        => DeleteEntity("testimonials", id, nameof(Testimonials), "Testimonial", ct);

    // ── Messages inbox (list / view / mark-read / delete) ──
    public async Task<IActionResult> Messages(CancellationToken ct)
    {
        var messages = await adminApi.GetMessagesAsync(ct);
        return View("Messages", messages.ToList());
    }

    [HttpGet]
    public async Task<IActionResult> MessageView(Guid id, CancellationToken ct)
    {
        var message = await adminApi.GetMessageAsync(id, ct);
        if (message is null) return NotFound();
        return View("MessageView", message);
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> MessageMarkRead(Guid id, CancellationToken ct)
    {
        var ok = await adminApi.MarkMessageReadAsync(id, ct);
        TempData["AdminMessage"] = ok ? "Marked as read." : "Could not update the message.";
        return RedirectToAction(nameof(Messages));
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> MessageDelete(Guid id, CancellationToken ct)
    {
        var ok = await adminApi.DeleteMessageAsync(id, ct);
        TempData["AdminMessage"] = ok ? "Message deleted." : "Could not delete the message.";
        return RedirectToAction(nameof(Messages));
    }

    // ── Profile (single bilingual upsert) ──
    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken ct)
    {
        var en = await publicApi.GetProfileAsync("en", ct);
        var fa = await publicApi.GetProfileAsync("fa", ct);
        return View("ProfileForm", AdminProfileMapper.ToFormModel(en, fa));
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("ProfileForm", model);
        var ok = await adminApi.UpsertProfileAsync(AdminProfileMapper.ToRequest(model), ct);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "Could not save the profile.");
            return View("ProfileForm", model);
        }
        TempData["AdminMessage"] = "Profile saved.";
        return RedirectToAction(nameof(Profile));
    }

    // ── ImpactMetric (real CRUD) ──
    public async Task<IActionResult> Metrics(CancellationToken ct)
    {
        var items = await crud.ListAsync<ImpactMetricApiDto>("impact-metrics", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Impact Metrics", Subtitle = "Experience-page metrics", SidebarKey = "metrics",
            CreateAction = nameof(MetricCreate), EditAction = nameof(MetricEdit), DeleteAction = nameof(MetricDelete),
            CreateLabel = "New Metric", Headers = ["Value", "Tag", "Color"], ShowStatusColumn = true,
            Rows = items.Select(AdminImpactMetricMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult MetricCreate() => View("MetricForm", new ImpactMetricFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> MetricCreate(ImpactMetricFormModel m, CancellationToken ct)
        => CreateEntity("impact-metrics", m, "MetricForm", nameof(Metrics), AdminImpactMetricMapper.ToRequest, "Metric", ct);
    [HttpGet] public Task<IActionResult> MetricEdit(Guid id, CancellationToken ct)
        => EditForm<ImpactMetricApiDto, ImpactMetricFormModel>("impact-metrics", id, "MetricForm", AdminImpactMetricMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> MetricEdit(Guid id, ImpactMetricFormModel m, CancellationToken ct)
        => UpdateEntity("impact-metrics", id, m, "MetricForm", nameof(Metrics), AdminImpactMetricMapper.ToRequest, "Metric", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> MetricDelete(Guid id, CancellationToken ct)
        => DeleteEntity("impact-metrics", id, nameof(Metrics), "Metric", ct);

    // ── Principle (real CRUD) ──
    public async Task<IActionResult> Principles(CancellationToken ct)
    {
        var items = await crud.ListAsync<PrincipleApiDto>("principles", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Principles", Subtitle = "Experience-page core principles", SidebarKey = "principles",
            CreateAction = nameof(PrincipleCreate), EditAction = nameof(PrincipleEdit), DeleteAction = nameof(PrincipleDelete),
            CreateLabel = "New Principle", Headers = ["Title", "Description"], ShowStatusColumn = true,
            Rows = items.Select(AdminPrincipleMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult PrincipleCreate() => View("PrincipleForm", new PrincipleFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> PrincipleCreate(PrincipleFormModel m, CancellationToken ct)
        => CreateEntity("principles", m, "PrincipleForm", nameof(Principles), AdminPrincipleMapper.ToRequest, "Principle", ct);
    [HttpGet] public Task<IActionResult> PrincipleEdit(Guid id, CancellationToken ct)
        => EditForm<PrincipleApiDto, PrincipleFormModel>("principles", id, "PrincipleForm", AdminPrincipleMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> PrincipleEdit(Guid id, PrincipleFormModel m, CancellationToken ct)
        => UpdateEntity("principles", id, m, "PrincipleForm", nameof(Principles), AdminPrincipleMapper.ToRequest, "Principle", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> PrincipleDelete(Guid id, CancellationToken ct)
        => DeleteEntity("principles", id, nameof(Principles), "Principle", ct);

    // ── ProficiencyGroup (real CRUD) ──
    public async Task<IActionResult> Proficiencies(CancellationToken ct)
    {
        var items = await crud.ListAsync<ProficiencyGroupApiDto>("proficiencies", "en", ct);
        return View("_AdminList", new AdminListViewModel
        {
            Title = "Proficiency Groups", Subtitle = "Experience-page proficiency matrix", SidebarKey = "proficiencies",
            CreateAction = nameof(ProficiencyCreate), EditAction = nameof(ProficiencyEdit), DeleteAction = nameof(ProficiencyDelete),
            CreateLabel = "New Group", Headers = ["Title", "Items", "Color"], ShowStatusColumn = true,
            Rows = items.Select(AdminProficiencyMapper.ToRow).ToList()
        });
    }

    [HttpGet] public IActionResult ProficiencyCreate() => View("ProficiencyForm", new ProficiencyFormModel());
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> ProficiencyCreate(ProficiencyFormModel m, CancellationToken ct)
        => CreateEntity("proficiencies", m, "ProficiencyForm", nameof(Proficiencies), AdminProficiencyMapper.ToRequest, "Proficiency group", ct);
    [HttpGet] public Task<IActionResult> ProficiencyEdit(Guid id, CancellationToken ct)
        => EditForm<ProficiencyGroupApiDto, ProficiencyFormModel>("proficiencies", id, "ProficiencyForm", AdminProficiencyMapper.ToFormModel, ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> ProficiencyEdit(Guid id, ProficiencyFormModel m, CancellationToken ct)
        => UpdateEntity("proficiencies", id, m, "ProficiencyForm", nameof(Proficiencies), AdminProficiencyMapper.ToRequest, "Proficiency group", ct);
    [HttpPost][ValidateAntiForgeryToken] public Task<IActionResult> ProficiencyDelete(Guid id, CancellationToken ct)
        => DeleteEntity("proficiencies", id, nameof(Proficiencies), "Proficiency group", ct);

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

    // ── Change password (authenticated admin changes their own password) ──
    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        var ok = await adminApi.ChangePasswordAsync(model.CurrentPassword, model.NewPassword, ct);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "Current password is incorrect.");
            return View(model);
        }

        TempData["AdminMessage"] = "Password changed.";
        return RedirectToAction(nameof(Index));
    }

    // Forgot-password: there is no email-based reset (single-admin app). This page
    // explains the real recovery path (change password while signed in).
    [AllowAnonymous]
    public IActionResult ForgotPassword() => View();
}
