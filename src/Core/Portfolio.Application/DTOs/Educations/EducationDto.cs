namespace Portfolio.Application.DTOs.Educations;

public class EducationDto
{
    public Guid Id { get; set; }
    public string InstitutionLogo { get; set; } = string.Empty;
    public string InstitutionUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public double? Gpa { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public string? Description { get; set; }
}