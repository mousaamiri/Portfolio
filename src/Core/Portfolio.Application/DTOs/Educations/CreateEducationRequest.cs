namespace Portfolio.Application.DTOs.Educations;

public class CreateEducationRequest
{
    public string InstitutionLogo { get; set; } = string.Empty;
    public string InstitutionUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public double? Gpa { get; set; }
    public List<EducationTranslationRequest> Translations { get; set; } = [];
}