namespace Portfolio.Application.DTOs.Experiences;

public class ExperienceDto
{
    public Guid Id { get; set; }
    public string CompanyLogo { get; set; } = string.Empty;
    public string CompanyUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
}