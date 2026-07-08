using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Faqs;

public class UpdateFaqRequest
{
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<FaqTranslationRequest> Translations { get; set; } = [];
}
