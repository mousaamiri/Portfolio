namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// Aggregate model for the About page (9 sections). Repeating sections are
/// driven by the lists below; fixed chrome/labels stay in the view with
/// data-i18n keys. Bio stat VALUES are English here + rendered with data-i18n
/// keys so translations.js supplies Farsi at runtime.
/// </summary>
public class AboutViewModel
{
    // Bio stat values (labels + age live in the view / JS).
    public string RoleValue { get; init; } = string.Empty;
    public string ExperienceValue { get; init; } = string.Empty;
    public string DegreeValue { get; init; } = string.Empty;

    public string PortraitUrl { get; init; } = "/images/about-portrait.jpg";
    public string PortraitAlt { get; init; } = "Mousa — portrait photo";

    public List<TimelineEntryViewModel> Journey { get; init; } = [];
    public List<StatCounterViewModel> Footprint { get; init; } = [];
    public List<SkillViewModel> Skills { get; init; } = [];
    public List<InterestViewModel> Interests { get; init; } = [];
    public List<EndorsementViewModel> Endorsements { get; init; } = [];
    public List<EducationViewModel> Education { get; init; } = [];
}
