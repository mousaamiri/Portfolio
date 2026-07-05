namespace Portfolio.Web.Models.ViewModels;

public class ProjectViewModel
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Technologies { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? DemoUrl { get; init; }
    public string? SourceCodeUrl { get; init; }
}
