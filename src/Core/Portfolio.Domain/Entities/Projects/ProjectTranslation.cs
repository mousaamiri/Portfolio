using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Projects;

public class ProjectTranslation : BaseTranslation
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Technologies { get; set; }

    public Project Project { get; set; } = null!;
}
