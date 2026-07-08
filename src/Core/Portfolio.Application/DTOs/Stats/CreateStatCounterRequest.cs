using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Stats;

public class CreateStatCounterRequest
{
    public string Icon { get; set; } = string.Empty;
    public long CountTarget { get; set; }
    public string? Suffix { get; set; }
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<StatCounterTranslationRequest> Translations { get; set; } = [];
}
