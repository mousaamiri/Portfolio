namespace Portfolio.Application.DTOs.Projects;

public class ProjectTranslationRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Technologies { get; set; }
}