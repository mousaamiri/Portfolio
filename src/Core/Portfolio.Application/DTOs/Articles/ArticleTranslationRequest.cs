using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Articles;

public class ArticleTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? Body { get; set; }
}
