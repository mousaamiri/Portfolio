namespace Portfolio.Application.DTOs.Skills;

public class SkillDto
{
    public Guid Id { get; set; }
    public string IconUrl { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
}
