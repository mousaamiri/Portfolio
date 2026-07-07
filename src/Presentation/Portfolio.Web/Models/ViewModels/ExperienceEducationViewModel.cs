namespace Portfolio.Web.Models.ViewModels;

/// <summary>One entry in the Experience-page education timeline. Distinct from
/// the public About Education section — this is the CV-style academic history
/// (degree, year range, institution, score, optional course tags).</summary>
public class ExperienceEducationViewModel
{
    public string TitleKey { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Years { get; init; } = string.Empty;          // static, e.g. "2019 – 2023"
    public string InstitutionKey { get; init; } = string.Empty;
    public string Institution { get; init; } = string.Empty;
    public string Score { get; init; } = string.Empty;          // static, e.g. "CGPA: 7.76"
    public bool IsLast { get; init; }
    public List<KeyedTextViewModel> Courses { get; init; } = [];
}
