using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.UiTranslations;

/// <summary>Create/update payload for a UI-chrome translation row.</summary>
public class UpsertUiTranslationRequest
{
    [Required]
    [RegularExpression(@"^[a-z]+\.[a-z_0-9]+$", ErrorMessage = "Key must be dot-notation, e.g. nav.home.")]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string LanguageCode { get; set; } = "fa";

    [Required]
    public string Value { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
