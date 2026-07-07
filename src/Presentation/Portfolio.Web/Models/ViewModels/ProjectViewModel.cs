namespace Portfolio.Web.Models.ViewModels;

/// <summary>One colored tech pill on the Work detail panel ({name, color}).</summary>
public class TechPillViewModel
{
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = "#3b82f6";
}

public class ProjectViewModel
{
    // ── Legacy fields (kept so _ProjectCard.cshtml + Home test still compile) ──
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Technologies { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? DemoUrl { get; init; }
    public string? SourceCodeUrl { get; init; }

    // ── Work master/detail fields ──
    // Work is the one page whose source data is genuine parallel EN/FA arrays
    // (not data-i18n), so the model carries both languages; work.js switches on
    // window.i18n.lang(). The displayed number uses DisplayId (01..06).
    public int DisplayId { get; init; }
    public string NameEn { get; init; } = string.Empty;
    public string NameFa { get; init; } = string.Empty;
    public string SubtitleEn { get; init; } = string.Empty;
    public string SubtitleFa { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public string DescriptionFa { get; init; } = string.Empty;
    public string GithubUrl { get; init; } = "#";
    public List<TechPillViewModel> Techs { get; init; } = [];
}
