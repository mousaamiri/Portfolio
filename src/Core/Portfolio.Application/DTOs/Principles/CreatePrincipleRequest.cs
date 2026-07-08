using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Principles;

public class CreatePrincipleRequest
{
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<PrincipleTranslationRequest> Translations { get; set; } = [];
}
