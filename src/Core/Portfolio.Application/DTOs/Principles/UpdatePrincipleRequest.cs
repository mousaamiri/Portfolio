using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Principles;

public class UpdatePrincipleRequest
{
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<PrincipleTranslationRequest> Translations { get; set; } = [];
}
