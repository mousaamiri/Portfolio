namespace Portfolio.Web.Models.ViewModels;

public class SkillViewModel
{
    public string Name { get; init; } = string.Empty;
    public string? Category { get; init; }
    public int Proficiency { get; init; }
    public string IconClass { get; init; } = string.Empty;
}
