namespace Portfolio.Application.DTOs.Projects;

public class CreateProjectRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? DemoUrl { get; set; }
    public string? SourceCodeUrl { get; set; }
    public int DisplayOrder { get; set; }
    public List<ProjectTranslationRequest> Translations { get; set; } = [];
}