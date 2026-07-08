using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Articles;

public class ArticleTranslation : BaseTranslation
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? Body { get; set; }

    public Article Article { get; set; } = null!;
}
