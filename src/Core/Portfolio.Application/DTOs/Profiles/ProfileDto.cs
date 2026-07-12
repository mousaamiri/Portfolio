namespace Portfolio.Application.DTOs.Profiles;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TelegramUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? ResumeUrlEn { get; set; }
    public string? ResumeUrlFa { get; set; }
    public string? PortraitUrl { get; set; }
    public string? LearningDate { get; set; }
    public string? Phone { get; set; }
    public string? CountryCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? Tagline { get; set; }
    public string? Bio { get; set; }
    public string? LearningTitle { get; set; }
    public string? LearningDesc { get; set; }
    public string? RoleBadge { get; set; }
    public string? ExperienceBadge { get; set; }
    public string? DegreeBadge { get; set; }
    public string? PortraitAlt { get; set; }
    public string? Location { get; set; }
    public string? Country { get; set; }
}
