using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Proficiencies;

public class UpdateProficiencyGroupRequest
{
    public string Color { get; set; } = "amber";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<ProficiencyGroupTranslationRequest> Translations { get; set; } = [];
}
