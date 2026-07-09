using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.Models.Admin;

// ── Experience ──
public class ExperienceFormModel
{
    public Guid? Id { get; set; }
    [Display(Name = "Company logo URL")] public string CompanyLogo { get; set; } = string.Empty;
    [Display(Name = "Company URL")] public string CompanyUrl { get; set; } = string.Empty;
    [Display(Name = "Start date")][DataType(DataType.Date)] public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    [Display(Name = "End date")][DataType(DataType.Date)] public DateTime? EndDate { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Company (EN)")] public string CompanyNameEn { get; set; } = string.Empty;
    [Required][Display(Name = "Job title (EN)")] public string JobTitleEn { get; set; } = string.Empty;
    [Display(Name = "Description (EN)")] public string? DescriptionEn { get; set; }
    [Display(Name = "Location (EN)")] public string? LocationEn { get; set; }

    [Display(Name = "Company (FA)")] public string CompanyNameFa { get; set; } = string.Empty;
    [Display(Name = "Job title (FA)")] public string JobTitleFa { get; set; } = string.Empty;
    [Display(Name = "Description (FA)")] public string? DescriptionFa { get; set; }
    [Display(Name = "Location (FA)")] public string? LocationFa { get; set; }

    public bool IsEdit => Id.HasValue;
}

// ── Education ──
public class EducationFormModel
{
    public Guid? Id { get; set; }
    [Display(Name = "Institution logo URL")] public string InstitutionLogo { get; set; } = string.Empty;
    [Display(Name = "Institution URL")] public string InstitutionUrl { get; set; } = string.Empty;
    [Display(Name = "Start date")][DataType(DataType.Date)] public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    [Display(Name = "End date")][DataType(DataType.Date)] public DateTime? EndDate { get; set; }
    [Display(Name = "GPA")] public double? Gpa { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Institution (EN)")] public string InstitutionNameEn { get; set; } = string.Empty;
    [Required][Display(Name = "Degree (EN)")] public string DegreeEn { get; set; } = string.Empty;
    [Required][Display(Name = "Field of study (EN)")] public string FieldOfStudyEn { get; set; } = string.Empty;
    [Display(Name = "Description (EN)")] public string? DescriptionEn { get; set; }

    [Display(Name = "Institution (FA)")] public string InstitutionNameFa { get; set; } = string.Empty;
    [Display(Name = "Degree (FA)")] public string DegreeFa { get; set; } = string.Empty;
    [Display(Name = "Field of study (FA)")] public string FieldOfStudyFa { get; set; } = string.Empty;
    [Display(Name = "Description (FA)")] public string? DescriptionFa { get; set; }

    public bool IsEdit => Id.HasValue;
}
