namespace Portfolio.Application.DTOs.Stats;

public class StatCounterDto
{
    public Guid Id { get; set; }
    public string Icon { get; set; } = string.Empty;
    public long CountTarget { get; set; }
    public string? Suffix { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? TipText { get; set; }
    public string? TipAriaLabel { get; set; }
}
