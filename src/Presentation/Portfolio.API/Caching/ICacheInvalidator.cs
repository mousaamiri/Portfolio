using StackExchange.Redis;

namespace Portfolio.API.Caching;

// Evicts cached public API responses. The same Redis-backed store that holds the
// entries implements this, so a purge and a store can never disagree about where an
// entry lives. Tags come straight from [OutputCache(Tags = ...)] on the public
// controllers, and [InvalidateCache] on the admin controllers triggers the purge.
public interface ICacheInvalidator
{
    // Evicts every cached entry tagged with <paramref name="tag"/> — one resource's
    // list and by-id endpoints across all languages in a single round trip.
    ValueTask InvalidateAsync(string tag, CancellationToken cancellationToken);

    // Evicts every cached public response — used when data embedded on every page
    // (the profile) changes.
    ValueTask InvalidateAllAsync(CancellationToken cancellationToken);
}
