namespace Portfolio.Domain.Common;

public interface ITranslatable<TTranslation> where TTranslation : BaseTranslation
{
    ICollection<TTranslation> Translations { get; set; }
}
