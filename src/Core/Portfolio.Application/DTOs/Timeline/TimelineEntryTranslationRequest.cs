using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Timeline;

public class TimelineEntryTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
