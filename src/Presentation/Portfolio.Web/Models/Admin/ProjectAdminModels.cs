using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.Models.Admin;

/// <summary>One row in the admin Projects list table.</summary>
public class ProjectListItem
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Technologies { get; init; }
    public bool IsPublished { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Create/Edit form for a Project. Bilingual text is flattened into EN/FA fields
/// (mapped to two translations on submit); everything else mirrors the entity.
/// </summary>
public class ProjectFormModel
{
    public Guid? Id { get; set; }

    [Required]
    [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must be lowercase words separated by hyphens.")]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Thumbnail URL")]
    public string ThumbnailUrl { get; set; } = string.Empty;

    [Display(Name = "Cover Image URL")]
    public string? CoverImageUrl { get; set; }

    [Display(Name = "Preview URL")]
    public string? PreviewUrl { get; set; }

    [Display(Name = "Source Code URL")]
    public string? SourceCodeUrl { get; set; }

    [Display(Name = "Source is private")]
    public bool IsSourcePrivate { get; set; }

    [Display(Name = "Client project")]
    public bool IsClientProject { get; set; }

    [Display(Name = "Featured")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Published")]
    public bool IsPublished { get; set; } = true;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Started")]
    [DataType(DataType.Date)]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow.Date;

    [Display(Name = "Completed")]
    [DataType(DataType.Date)]
    public DateTime? CompletedAt { get; set; }

    [Display(Name = "Display order")]
    public int DisplayOrder { get; set; }

    // ── English ──
    [Required]
    [Display(Name = "Title (EN)")]
    public string TitleEn { get; set; } = string.Empty;
    [Display(Name = "Short description (EN)")]
    public string ShortDescriptionEn { get; set; } = string.Empty;
    [Display(Name = "Description (EN)")]
    public string? DescriptionEn { get; set; }
    [Display(Name = "Technologies (EN)")]
    public string? TechnologiesEn { get; set; }

    // ── Persian ──
    [Display(Name = "Title (FA)")]
    public string TitleFa { get; set; } = string.Empty;
    [Display(Name = "Short description (FA)")]
    public string ShortDescriptionFa { get; set; } = string.Empty;
    [Display(Name = "Description (FA)")]
    public string? DescriptionFa { get; set; }
    [Display(Name = "Technologies (FA)")]
    public string? TechnologiesFa { get; set; }

    public bool IsEdit => Id.HasValue;
}
