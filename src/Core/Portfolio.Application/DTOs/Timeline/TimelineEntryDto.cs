namespace Portfolio.Application.DTOs.Timeline;

public class TimelineEntryDto
{
    public Guid Id { get; set; }
    public string Year { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
