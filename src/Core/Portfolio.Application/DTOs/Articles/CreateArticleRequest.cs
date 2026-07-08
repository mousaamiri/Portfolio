using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Articles;

public class CreateArticleRequest
{
    [Required]
    public string Slug { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime PublishDate { get; set; }
    public int ReadTimeMinutes { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<ArticleTranslationRequest> Translations { get; set; } = [];
}
