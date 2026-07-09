using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.Models.Admin;

public class SkillListItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Category { get; init; }
    public int Proficiency { get; init; }
    public bool IsActive { get; init; }
}

public class SkillFormModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Icon (devicon name)")]
    public string IconUrl { get; set; } = string.Empty;

    [Range(0, 100)]
    public int Proficiency { get; set; } = 70;

    [Display(Name = "Display order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    // English
    [Required]
    [Display(Name = "Name (EN)")]
    public string NameEn { get; set; } = string.Empty;
    [Display(Name = "Category (EN)")]
    public string? CategoryEn { get; set; }
    [Display(Name = "Description (EN)")]
    public string? DescriptionEn { get; set; }

    // Persian
    [Display(Name = "Name (FA)")]
    public string NameFa { get; set; } = string.Empty;
    [Display(Name = "Category (FA)")]
    public string? CategoryFa { get; set; }
    [Display(Name = "Description (FA)")]
    public string? DescriptionFa { get; set; }

    public bool IsEdit => Id.HasValue;
}
