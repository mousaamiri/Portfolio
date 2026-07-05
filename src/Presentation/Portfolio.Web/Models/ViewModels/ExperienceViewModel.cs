namespace Portfolio.Web.Models.ViewModels;

public class ExperienceViewModel
{
    public string CompanyName { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Location { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
