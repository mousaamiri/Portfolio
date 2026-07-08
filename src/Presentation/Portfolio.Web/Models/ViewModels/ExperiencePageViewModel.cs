namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// Aggregate model for the Experience page. Purpose-built for the static
/// site's rich layout (metrics, principles, summary, CV education, a single
/// current role with bullet points + stack, and a proficiency matrix). The
/// legacy <see cref="ExperienceViewModel"/> (a simple job card) is left intact
/// for compatibility but is not used to render this page.
/// </summary>
public class ExperiencePageViewModel
{
    // Impact metrics + core principles come from Portfolio.API (settable so the
    // controller can attach them onto the otherwise mock-built aggregate).
    public List<ImpactMetricViewModel> Metrics { get; set; } = [];
    public List<PrincipleViewModel> Principles { get; set; } = [];

    // Real professional history from Portfolio.API (settable so the controller can
    // attach it onto the otherwise mock-built aggregate). When empty, the view
    // falls back to the single static role block below.
    public List<ExperienceViewModel> ProfessionalHistory { get; set; } = [];

    // Summary
    public string SummaryName { get; init; } = string.Empty;        // static
    public string SummaryTextKey { get; init; } = "exp.summary_text";
    public string SummaryText { get; init; } = string.Empty;

    // CV-style education timeline
    public List<ExperienceEducationViewModel> Education { get; init; } = [];

    // Single current professional role
    public string JobTitleKey { get; init; } = "exp.job_title";
    public string JobTitle { get; init; } = string.Empty;
    public string JobDateKey { get; init; } = "exp.job_date";
    public string JobDate { get; init; } = string.Empty;            // e.g. "2024 – Present"
    public string JobCompanyKey { get; init; } = "exp.job_company";
    public string JobCompany { get; init; } = string.Empty;
    public List<KeyedTextViewModel> JobBullets { get; init; } = []; // Text may contain inline highlight HTML
    public List<string> Stack { get; init; } = [];                  // static pills

    // Proficiency matrix (name lists, not percentages) — from Portfolio.API.
    public List<ProficiencyGroupViewModel> Proficiency { get; set; } = [];
}
