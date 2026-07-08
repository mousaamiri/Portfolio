using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Interests;

public class CreateInterestRequest
{
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<InterestTranslationRequest> Translations { get; set; } = [];
}
