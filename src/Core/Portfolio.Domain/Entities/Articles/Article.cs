using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Articles;

public class Article : BaseEntity, ITranslatable<ArticleTranslation>
{
    public string Slug { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime PublishDate { get; set; }
    public int ReadTimeMinutes { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<ArticleTranslation> Translations { get; set; } = [];
}
