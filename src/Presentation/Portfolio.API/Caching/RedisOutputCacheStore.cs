using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Portfolio.API.Caching;

// Redis-backed <see cref="IOutputCacheStore"/> for ASP.NET Core's output-caching
// middleware, with tag-based eviction for admin writes.
//
// The middleware owns cache-key generation (scheme/host/port/path/query, plus the
// VaryByQueryKeys from [OutputCache]) and serializes the response itself — this layer
// only stores the opaque payload bytes. Entries live under
// "{InstanceName}:oc:entry:{key-hash}" and every entry is indexed in the Redis set
// "{InstanceName}:oc:tag:{tag}" for each tag declared by the action, so one admin
// write can evict every cached variant of a resource (all languages, list + by-id)
// atomically in a single round trip.
//
// Redis is optional: with no connection configured/reachable, every operation is a
// no-op and the site serves uncached.
internal sealed class RedisOutputCacheStore : IOutputCacheStore, ICacheInvalidator
{
    private const string EntryPrefix = "oc:entry:";
    private const string MetaPrefix = "oc:meta:";
    private const string TagPrefix = "oc:tag:";

    // Tag carried by every cached public response so a profile change can evict the
    // whole public cache via <see cref="InvalidateAllAsync"/>.
    public const string AllTag = "_all";

    private static readonly char[] TagSeparator = ['\n'];

    // Evicts a tag's members and their metadata in one atomic, single-round-trip
    // script: SMEMBERS then a DEL per entry, so a concurrent SetAsync cannot leave a
    // just-cached response behind.
    private static readonly LuaScript EvictByTagScript = LuaScript.Prepare("""
        local keys = redis.call('SMEMBERS', @tagKey)
        local deleted = 0
        for _, key in ipairs(keys) do
            deleted = deleted + redis.call('DEL', @entryPrefix .. key)
            deleted = deleted + redis.call('DEL', @metaPrefix .. key)
        end
        deleted = deleted + redis.call('DEL', @tagKey)
        return deleted
        """);

    private readonly RedisConnectionProvider _connection;
    private readonly ILogger<RedisOutputCacheStore> _logger;
    private readonly string _instanceName;
    private int _logUnavailableSuppressed;

    public RedisOutputCacheStore(
        RedisConnectionProvider connection,
        IOptions<RedisOutputCacheOptions> options,
        ILogger<RedisOutputCacheStore> logger)
    {
        _connection = connection;
        _logger = logger;
        _instanceName = string.IsNullOrWhiteSpace(options.Value.InstanceName)
            ? "portfolio"
            : options.Value.InstanceName.Trim('/');
    }

    /// <inheritdoc />
    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var db = _connection.TryGetDatabase();
        if (db is null)
            return null;

        try
        {
            var value = await db.StringGetAsync(EntryKey(key));
            return value.HasValue ? (byte[]?)value : null;
        }
        catch (Exception ex) when (IsTransient(ex))
        {
            LogUnavailableOnce(ex);
            return null;
        }
    }

    /// <inheritdoc />
    public async ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        var db = _connection.TryGetDatabase();
        if (db is null)
            return;

        // Untagged entries only need storing (none of the public endpoints are untagged).
        if (tags is null || tags.Length == 0)
        {
            try
            {
                await db.StringSetAsync(EntryKey(key), value, expiry: validFor);
            }
            catch (Exception ex) when (IsTransient(ex))
            {
                LogUnavailableOnce(ex);
            }

            return;
        }

        try
        {
            // StackExchange.Redis pipelines commands issued without awaiting between
            // them, so the entry, its tag memberships and its tag record land in Redis
            // in a single round trip.
            var metaKey = MetaKey(key);
            var tasks = new List<Task>(tags.Length + 2)
            {
                db.StringSetAsync(EntryKey(key), value, expiry: validFor),
                // The side record of an entry's tags backs EvictAsync; it expires with
                // the entry so a key that dies on TTL takes its bookkeeping with it
                // (tag sets keep only stale *names* until the next purge).
                db.StringSetAsync(metaKey, Encoding.UTF8.GetBytes(string.Join('\n', tags)), expiry: validFor),
                db.SetAddAsync(TagKey(AllTag), key)
            };

            foreach (var tag in tags)
                tasks.Add(db.SetAddAsync(TagKey(tag), key));

            await Task.WhenAll(tasks);
        }
        catch (Exception ex) when (IsTransient(ex))
        {
            LogUnavailableOnce(ex);
        }
    }

    /// <inheritdoc />
    public async ValueTask EvictAsync(string key, CancellationToken cancellationToken)
    {
        var db = _connection.TryGetDatabase();
        if (db is null)
            return;

        try
        {
            var metaKey = MetaKey(key);
            var meta = await db.StringGetAsync(metaKey);

            var tasks = new List<Task>(3)
            {
                db.KeyDeleteAsync(EntryKey(key)),
                db.KeyDeleteAsync(metaKey)
            };

            if (meta.HasValue)
            {
                foreach (var tag in Encoding.UTF8.GetString(meta!).Split(TagSeparator, StringSplitOptions.RemoveEmptyEntries))
                    tasks.Add(db.SetRemoveAsync(TagKey(tag), key));
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex) when (IsTransient(ex))
        {
            LogUnavailableOnce(ex);
        }
    }

    /// <inheritdoc />
    public ValueTask InvalidateAsync(string tag, CancellationToken cancellationToken)
        => EvictByTagAsync(tag, cancellationToken);

    /// <inheritdoc />
    public ValueTask InvalidateAllAsync(CancellationToken cancellationToken)
        => EvictByTagAsync(AllTag, cancellationToken);

    /// <inheritdoc />
    public async ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        var db = _connection.TryGetDatabase();
        if (db is null)
            return;

        try
        {
            var result = await db.ScriptEvaluateAsync(EvictByTagScript, new
            {
                tagKey = (RedisKey)TagKey(tag),
                entryPrefix = (RedisValue)($"{_instanceName}:{EntryPrefix}"),
                metaPrefix = (RedisValue)($"{_instanceName}:{MetaPrefix}")
            });

            _logger.LogDebug("Evicted {Count} cached response(s) tagged '{Tag}'.", (long)result, tag);
        }
        catch (Exception ex) when (IsTransient(ex))
        {
            LogUnavailableOnce(ex);
        }
    }

    // The middleware's key is opaque (and long, with query strings embedded); hashing
    // keeps Redis keys compact and binary-safe.
    private RedisKey EntryKey(string key)
        => $"{_instanceName}:{EntryPrefix}{HashKey(key)}";

    private RedisKey MetaKey(string key)
        => $"{_instanceName}:{MetaPrefix}{HashKey(key)}";

    private RedisKey TagKey(string tag)
        => $"{_instanceName}:{TagPrefix}{tag}";

    private static string HashKey(string key)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)));

    private static bool IsTransient(Exception ex)
        => ex is RedisException or TimeoutException;

    private void LogUnavailableOnce(Exception ex)
    {
        if (Interlocked.Exchange(ref _logUnavailableSuppressed, 1) == 0)
            _logger.LogWarning(ex, "Redis output cache is unavailable; serving uncached responses.");
    }
}
