namespace Portfolio.Application.DTOs.Skills;

public class CreateSkillRequest
{
    public string IconUrl { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }
    public List<SkillTranslationRequest> Translations { get; set; } = [];
}
