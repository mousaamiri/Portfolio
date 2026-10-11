namespace Portfolio.API.Caching;

// Output-cache settings, bound from the "Redis" configuration section. Accepts any
// StackExchange.Redis configuration string (e.g. "localhost:6379" or
// "my-redis.redis.cache.windows.net:6380,password=...,ssl=True,abortConnect=False").
// Empty = caching disabled: the app serves uncached and works normally.
public sealed class RedisOutputCacheOptions
{
    public const string SectionName = "Redis";

    public string Configuration { get; set; } = string.Empty;

    // Logical partition so one shared Redis instance can serve several apps.
    public string InstanceName { get; set; } = "portfolio";

    // How long a cached public response stays valid. The plan's budget is 5 minutes;
    // admin writes evict proactively, so this only bounds staleness if a purge fails.
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);
}
