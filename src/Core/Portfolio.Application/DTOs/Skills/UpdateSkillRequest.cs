using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Skills;

public class UpdateSkillRequest
{
    [Required]
    public string IconUrl { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<SkillTranslationRequest> Translations { get; set; } = [];
}
