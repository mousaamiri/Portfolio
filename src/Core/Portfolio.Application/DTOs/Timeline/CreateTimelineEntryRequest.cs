using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Timeline;

public class CreateTimelineEntryRequest
{
    [Required]
    public string Year { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<TimelineEntryTranslationRequest> Translations { get; set; } = [];
}
