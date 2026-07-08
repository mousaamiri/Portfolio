namespace Portfolio.Application.DTOs.Principles;

public class PrincipleDto
{
    public Guid Id { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
