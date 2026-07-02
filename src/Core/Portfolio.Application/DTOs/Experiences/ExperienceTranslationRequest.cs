namespace Portfolio.Application.DTOs.Experiences;

public class ExperienceTranslationRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
}