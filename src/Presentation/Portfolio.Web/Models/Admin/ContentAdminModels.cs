using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.Models.Admin;

// ── Article ──
public class ArticleFormModel
{
    public Guid? Id { get; set; }
    [Required]
    [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must be lowercase words separated by hyphens.")]
    public string Slug { get; set; } = string.Empty;
    [Display(Name = "Category")] public string? Category { get; set; }
    [Display(Name = "Tags (comma-separated)")] public string? Tags { get; set; }
    [Display(Name = "Cover image URL")] public string? CoverImageUrl { get; set; }
    [Display(Name = "Publish date")][DataType(DataType.Date)] public DateTime PublishDate { get; set; } = DateTime.UtcNow.Date;
    [Display(Name = "Read time (min)")] public int ReadTimeMinutes { get; set; } = 3;
    [Display(Name = "Published")] public bool IsPublished { get; set; } = true;
    [Display(Name = "Display order")] public int DisplayOrder { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Title (EN)")] public string TitleEn { get; set; } = string.Empty;
    [Display(Name = "Excerpt (EN)")] public string? ExcerptEn { get; set; }
    [Display(Name = "Body HTML (EN)")] public string? BodyEn { get; set; }

    [Display(Name = "Title (FA)")] public string TitleFa { get; set; } = string.Empty;
    [Display(Name = "Excerpt (FA)")] public string? ExcerptFa { get; set; }
    [Display(Name = "Body HTML (FA)")] public string? BodyFa { get; set; }

    public bool IsEdit => Id.HasValue;
}

// ── Testimonial ──
public class TestimonialFormModel
{
    public Guid? Id { get; set; }
    [Display(Name = "Initials")] public string Initials { get; set; } = string.Empty;
    [Display(Name = "Avatar color")] public string AvatarColor { get; set; } = "var(--accent)";
    [Display(Name = "Display order")] public int DisplayOrder { get; set; }
    [Display(Name = "Active")] public bool IsActive { get; set; } = true;

    [Required][Display(Name = "Quote (EN)")] public string QuoteEn { get; set; } = string.Empty;
    [Required][Display(Name = "Name (EN)")] public string NameEn { get; set; } = string.Empty;
    [Display(Name = "Role (EN)")] public string RoleEn { get; set; } = string.Empty;

    [Display(Name = "Quote (FA)")] public string QuoteFa { get; set; } = string.Empty;
    [Display(Name = "Name (FA)")] public string NameFa { get; set; } = string.Empty;
    [Display(Name = "Role (FA)")] public string RoleFa { get; set; } = string.Empty;

    public bool IsEdit => Id.HasValue;
}
