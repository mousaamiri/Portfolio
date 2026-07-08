using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Stats;

public class UpdateStatCounterRequest
{
    public string Icon { get; set; } = string.Empty;
    public long CountTarget { get; set; }
    public string? Suffix { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<StatCounterTranslationRequest> Translations { get; set; } = [];
}
