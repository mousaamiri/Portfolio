namespace Portfolio.Web.Models.ViewModels;

/// <summary>One card in the Experience "Technical Proficiency Matrix". This is
/// NOT the same data as the About skills grid (migration note, Step 5): it has
/// no percentages — just a titled, colored list of skill names.</summary>
public class ProficiencyGroupViewModel
{
    public string TitleKey { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Color { get; init; } = "amber";   // amber | pink | purple
    public List<string> Items { get; init; } = [];   // static skill names
}
