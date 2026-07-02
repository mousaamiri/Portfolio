namespace Portfolio.Application.DTOs.Projects;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? DemoUrl { get; set; }
    public string? SourceCodeUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Technologies { get; set; }
}