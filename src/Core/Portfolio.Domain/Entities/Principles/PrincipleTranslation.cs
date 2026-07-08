using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Principles;

public class PrincipleTranslation : BaseTranslation
{
    public Guid PrincipleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Principle Principle { get; set; } = null!;
}
