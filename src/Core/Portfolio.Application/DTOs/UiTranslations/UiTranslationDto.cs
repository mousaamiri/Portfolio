namespace Portfolio.Application.DTOs.UiTranslations;

/// <summary>A single UI-chrome translation row (flat key→value per language).</summary>
public class UiTranslationDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
