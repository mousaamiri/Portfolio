using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Proficiencies;

public class CreateProficiencyGroupRequest
{
    public string Color { get; set; } = "amber";
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<ProficiencyGroupTranslationRequest> Translations { get; set; } = [];
}
