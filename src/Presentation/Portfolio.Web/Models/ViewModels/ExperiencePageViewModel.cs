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
    public List<ImpactMetricViewModel> Metrics { get; init; } = [];
    public List<PrincipleViewModel> Principles { get; init; } = [];

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

    // Proficiency matrix (name lists, not percentages)
    public List<ProficiencyGroupViewModel> Proficiency { get; init; } = [];
}
