using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Common;

public abstract class BaseTranslation
{
    public Guid Id { get; set; }
    public Language Language { get; set; }
}
