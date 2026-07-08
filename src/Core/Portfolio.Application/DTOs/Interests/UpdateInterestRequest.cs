using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Interests;

public class UpdateInterestRequest
{
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<InterestTranslationRequest> Translations { get; set; } = [];
}
