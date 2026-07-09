using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.Models.Admin;

// ── ImpactMetric ──
public class ImpactMetricFormModel
{
    public Guid? Id { get; set; }
    [Required][Display(Name = "Value")] public string Value { get; set; } = string.Empty;
    [Display(Name = "Color (amber/pink/green)")] public string Color { get; set; } = "amber";
    [Display(Name = "Display order")] public int DisplayOrder { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Tag (EN)")] public string TagEn { get; set; } = string.Empty;
    [Display(Name = "Description (EN)")] public string DescriptionEn { get; set; } = string.Empty;
    [Display(Name = "Tag (FA)")] public string TagFa { get; set; } = string.Empty;
    [Display(Name = "Description (FA)")] public string DescriptionFa { get; set; } = string.Empty;

    public bool IsEdit => Id.HasValue;
}

// ── Principle ──
public class PrincipleFormModel
{
    public Guid? Id { get; set; }
    [Display(Name = "Display order")] public int DisplayOrder { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Title (EN)")] public string TitleEn { get; set; } = string.Empty;
    [Display(Name = "Description (EN)")] public string DescriptionEn { get; set; } = string.Empty;
    [Display(Name = "Title (FA)")] public string TitleFa { get; set; } = string.Empty;
    [Display(Name = "Description (FA)")] public string DescriptionFa { get; set; } = string.Empty;

    public bool IsEdit => Id.HasValue;
}

// ── ProficiencyGroup ──
public class ProficiencyFormModel
{
    public Guid? Id { get; set; }
    [Display(Name = "Color (amber/pink/purple)")] public string Color { get; set; } = "amber";
    [Display(Name = "Display order")] public int DisplayOrder { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Title (EN)")] public string TitleEn { get; set; } = string.Empty;
    [Display(Name = "Items (EN, comma-separated)")] public string ItemsEn { get; set; } = string.Empty;
    [Display(Name = "Title (FA)")] public string TitleFa { get; set; } = string.Empty;
    [Display(Name = "Items (FA, comma-separated)")] public string ItemsFa { get; set; } = string.Empty;

    public bool IsEdit => Id.HasValue;
}
