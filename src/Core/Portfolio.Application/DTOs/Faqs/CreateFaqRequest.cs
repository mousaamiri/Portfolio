using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Faqs;

public class CreateFaqRequest
{
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<FaqTranslationRequest> Translations { get; set; } = [];
}
