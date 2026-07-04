using Portfolio.Domain.Common;

namespace Portfolio.Application.Extensions;

public static class CollectionSyncExtensions
{
    public static void SyncTranslations<TTranslation>(
        this ICollection<TTranslation> existing,
        IReadOnlyList<TTranslation> incoming,
        Action<TTranslation, TTranslation> applyChanges)
        where TTranslation : BaseTranslation
    {
        var incomingLanguages = incoming.Select(i => i.Language).ToHashSet();

        var toRemove = existing
            .Where(e => !incomingLanguages.Contains(e.Language))
            .ToList();
        foreach (var item in toRemove)
            existing.Remove(item);

        foreach (var inc in incoming)
        {
            var exist = existing.FirstOrDefault(e => e.Language == inc.Language);
            if (exist != null)
                applyChanges(exist, inc);
            else
                existing.Add(inc);
        }
    }
}
