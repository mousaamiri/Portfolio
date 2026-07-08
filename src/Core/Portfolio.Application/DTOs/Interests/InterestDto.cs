namespace Portfolio.Application.DTOs.Interests;

public class InterestDto
{
    public Guid Id { get; set; }
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Label { get; set; } = string.Empty;
}
