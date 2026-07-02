using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Projects;

public class Project : BaseEntity, ITranslatable<ProjectTranslation>
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? DemoUrl { get; set; }
    public string? SourceCodeUrl { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<ProjectTranslation> Translations { get; set; } = [];
}
