namespace Portfolio.Application.DTOs.Proficiencies;

public class ProficiencyGroupDto
{
    public Guid Id { get; set; }
    public string Color { get; set; } = "amber";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;   // comma-separated skill names
}
