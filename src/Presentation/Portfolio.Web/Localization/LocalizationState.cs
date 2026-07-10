namespace Portfolio.Web.Localization;

/// <summary>
/// Per-request localization state, populated by <see cref="LanguageMiddleware"/>
/// from the language cookie. Read by the layout (html lang/dir + injected
/// <c>window.__ui</c>) and by the <c>@T</c> Razor helper. English carries an
/// empty <see cref="Map"/> — the views/JS fall back to their inline defaults.
/// </summary>
public class LocalizationState
{
    public string Language { get; set; } = Services.WebLanguage.Default;
    public IReadOnlyDictionary<string, string> Map { get; set; } = new Dictionary<string, string>();

    public bool IsFa => Language == "fa";
    public bool IsRtl => IsFa;
    public string Dir => IsRtl ? "rtl" : "ltr";
}
