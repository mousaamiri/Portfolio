namespace Portfolio.Web.Models.ViewModels;

/// <summary>Small (i18n key, text) pair. Text may contain inline HTML
/// (e.g. Experience job bullets with &lt;span class="highlight"&gt;).</summary>
public class KeyedTextViewModel
{
    public string Key { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
}
