namespace Portfolio.Application.DTOs.Projects;

public class UpdateProjectRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? DemoUrl { get; set; }
    public string? SourceCodeUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<ProjectTranslationRequest> Translations { get; set; } = [];
}