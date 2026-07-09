using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.Models.Admin;

// ── Faq ──
public class FaqFormModel
{
    public Guid? Id { get; set; }
    [Display(Name = "Display order")] public int DisplayOrder { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Question (EN)")] public string QuestionEn { get; set; } = string.Empty;
    [Required][Display(Name = "Answer (EN)")] public string AnswerEn { get; set; } = string.Empty;
    [Display(Name = "Question (FA)")] public string QuestionFa { get; set; } = string.Empty;
    [Display(Name = "Answer (FA)")] public string AnswerFa { get; set; } = string.Empty;

    public bool IsEdit => Id.HasValue;
}

// ── Interest ──
public class InterestFormModel
{
    public Guid? Id { get; set; }
    [Display(Name = "Icon (lucide name)")] public string Icon { get; set; } = string.Empty;
    [Display(Name = "Display order")] public int DisplayOrder { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Label (EN)")] public string LabelEn { get; set; } = string.Empty;
    [Display(Name = "Label (FA)")] public string LabelFa { get; set; } = string.Empty;

    public bool IsEdit => Id.HasValue;
}

// ── TimelineEntry ──
public class TimelineFormModel
{
    public Guid? Id { get; set; }
    [Required][Display(Name = "Year")] public string Year { get; set; } = string.Empty;
    [Display(Name = "Icon (lucide name)")] public string Icon { get; set; } = string.Empty;
    [Display(Name = "Display order")] public int DisplayOrder { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Title (EN)")] public string TitleEn { get; set; } = string.Empty;
    [Display(Name = "Description (EN)")] public string? DescriptionEn { get; set; }
    [Display(Name = "Title (FA)")] public string TitleFa { get; set; } = string.Empty;
    [Display(Name = "Description (FA)")] public string? DescriptionFa { get; set; }

    public bool IsEdit => Id.HasValue;
}

// ── StatCounter ──
public class StatFormModel
{
    public Guid? Id { get; set; }
    [Display(Name = "Icon (lucide name)")] public string Icon { get; set; } = string.Empty;
    [Display(Name = "Count target")] public long CountTarget { get; set; }
    [Display(Name = "Suffix")] public string? Suffix { get; set; }
    [Display(Name = "Display order")] public int DisplayOrder { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Label (EN)")] public string LabelEn { get; set; } = string.Empty;
    [Display(Name = "Tooltip (EN)")] public string? TipTextEn { get; set; }
    [Display(Name = "Label (FA)")] public string LabelFa { get; set; } = string.Empty;
    [Display(Name = "Tooltip (FA)")] public string? TipTextFa { get; set; }

    public bool IsEdit => Id.HasValue;
}
