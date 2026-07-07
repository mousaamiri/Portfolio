namespace Portfolio.Web.Models.ViewModels;

public class EducationViewModel
{
    public string InstitutionName { get; init; } = string.Empty;
    public string Degree { get; init; } = string.Empty;
    public string FieldOfStudy { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public double? Gpa { get; init; }

    // Raw score string as authored in the admin panel (e.g. "18.5/20",
    // Persian 20-point scale). Used by the public About Education section.
    public string? Score { get; init; }
}
