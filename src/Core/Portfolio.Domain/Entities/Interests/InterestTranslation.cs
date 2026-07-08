using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Interests;

public class InterestTranslation : BaseTranslation
{
    public Guid InterestId { get; set; }
    public string Label { get; set; } = string.Empty;

    public Interest Interest { get; set; } = null!;
}
