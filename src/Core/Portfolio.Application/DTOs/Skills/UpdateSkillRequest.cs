namespace Portfolio.Application.DTOs.Skills;

public class UpdateSkillRequest
{
    public string IconUrl { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<SkillTranslationRequest> Translations { get; set; } = [];
}
