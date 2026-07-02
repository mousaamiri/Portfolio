namespace Portfolio.Application.DTOs.Experiences;

public class CreateExperienceRequest
{
    public string CompanyLogo { get; set; } = string.Empty;
    public string CompanyUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<ExperienceTranslationRequest> Translations { get; set; } = [];
}