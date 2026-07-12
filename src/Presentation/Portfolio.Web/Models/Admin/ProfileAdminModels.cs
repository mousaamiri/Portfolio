using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.Models.Admin;

/// <summary>
/// Single-profile edit form (bilingual). There is one Profile row; the admin
/// action upserts it. Language-neutral contact/links fields plus EN/FA hero copy.
/// </summary>
public class ProfileFormModel
{
    [Required][EmailAddress] public string Email { get; set; } = string.Empty;
    [Display(Name = "GitHub URL")] public string? GitHubUrl { get; set; }
    [Display(Name = "LinkedIn URL")] public string? LinkedInUrl { get; set; }
    [Display(Name = "Instagram URL")] public string? InstagramUrl { get; set; }
    [Display(Name = "Telegram URL")] public string? TelegramUrl { get; set; }
    [Display(Name = "X (Twitter) URL")] public string? TwitterUrl { get; set; }
    [Display(Name = "Website URL")] public string? WebsiteUrl { get; set; }
    [Display(Name = "Resume URL (EN)")] public string? ResumeUrlEn { get; set; }
    [Display(Name = "Resume URL (FA)")] public string? ResumeUrlFa { get; set; }
    [Display(Name = "Portrait URL")] public string? PortraitUrl { get; set; }
    [Display(Name = "Learning date")] public string? LearningDate { get; set; }
    [Display(Name = "Phone")] public string? Phone { get; set; }
    [Display(Name = "Country code (ISO)")] public string? CountryCode { get; set; }

    // English
    [Required][Display(Name = "Full name (EN)")] public string FullNameEn { get; set; } = string.Empty;
    [Required][Display(Name = "Job title (EN)")] public string JobTitleEn { get; set; } = string.Empty;
    [Display(Name = "Tagline (EN)")] public string? TaglineEn { get; set; }
    [Display(Name = "Bio (EN)")] public string? BioEn { get; set; }
    [Display(Name = "Learning title (EN)")] public string? LearningTitleEn { get; set; }
    [Display(Name = "Learning desc (EN)")] public string? LearningDescEn { get; set; }
    [Display(Name = "Role badge (EN)")] public string? RoleBadgeEn { get; set; }
    [Display(Name = "Experience badge (EN)")] public string? ExperienceBadgeEn { get; set; }
    [Display(Name = "Degree badge (EN)")] public string? DegreeBadgeEn { get; set; }
    [Display(Name = "Portrait alt (EN)")] public string? PortraitAltEn { get; set; }
    [Display(Name = "Location (EN)")] public string? LocationEn { get; set; }
    [Display(Name = "Country (EN)")] public string? CountryEn { get; set; }

    // Persian
    [Display(Name = "Full name (FA)")] public string FullNameFa { get; set; } = string.Empty;
    [Display(Name = "Job title (FA)")] public string JobTitleFa { get; set; } = string.Empty;
    [Display(Name = "Tagline (FA)")] public string? TaglineFa { get; set; }
    [Display(Name = "Bio (FA)")] public string? BioFa { get; set; }
    [Display(Name = "Learning title (FA)")] public string? LearningTitleFa { get; set; }
    [Display(Name = "Learning desc (FA)")] public string? LearningDescFa { get; set; }
    [Display(Name = "Role badge (FA)")] public string? RoleBadgeFa { get; set; }
    [Display(Name = "Experience badge (FA)")] public string? ExperienceBadgeFa { get; set; }
    [Display(Name = "Degree badge (FA)")] public string? DegreeBadgeFa { get; set; }
    [Display(Name = "Portrait alt (FA)")] public string? PortraitAltFa { get; set; }
    [Display(Name = "Location (FA)")] public string? LocationFa { get; set; }
    [Display(Name = "Country (FA)")] public string? CountryFa { get; set; }
}
