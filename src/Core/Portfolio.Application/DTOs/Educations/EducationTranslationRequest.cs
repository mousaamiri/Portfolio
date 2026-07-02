namespace Portfolio.Application.DTOs.Educations;

public class EducationTranslationRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public string? Description { get; set; }
}