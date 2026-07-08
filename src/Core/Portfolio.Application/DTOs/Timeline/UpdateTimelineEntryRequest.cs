using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Timeline;

public class UpdateTimelineEntryRequest
{
    [Required]
    public string Year { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<TimelineEntryTranslationRequest> Translations { get; set; } = [];
}
