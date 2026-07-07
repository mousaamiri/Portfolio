namespace Portfolio.Web.Models.ViewModels;

/// <summary>One "Core Principle" on the Experience page (title + description,
/// both client-side i18n).</summary>
public class PrincipleViewModel
{
    public string TitleKey { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string DescKey { get; init; } = string.Empty;
    public string Desc { get; init; } = string.Empty;
}
