namespace Portfolio.Application.DTOs.ImpactMetrics;

public class ImpactMetricDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Color { get; set; } = "amber";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Tag { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
