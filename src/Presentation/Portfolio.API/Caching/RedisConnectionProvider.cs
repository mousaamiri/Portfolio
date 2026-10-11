using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Portfolio.API.Caching;

// Connection lifetime for the Redis output cache. Redis is an optional dependency:
// if it is unreachable the site keeps working (serving uncached), so the connection
// is established in the background at startup and every database access is a
// best-effort "try" that returns null while disconnected.
public sealed class RedisConnectionProvider : IHostedService, IDisposable
{
    private readonly IOptions<RedisOutputCacheOptions> _options;
    private readonly ILogger<RedisConnectionProvider> _logger;
    private ConnectionMultiplexer? _multiplexer;
    private bool _disposed;

    public RedisConnectionProvider(IOptions<RedisOutputCacheOptions> options, ILogger<RedisConnectionProvider> logger)
    {
        _options = options;
        _logger = logger;
    }

    // Connected in the background (see StartAsync) so a slow or dead Redis endpoint
    // never delays startup or request handling.
    public IDatabase? TryGetDatabase()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // IsConnected is false until the multiplexer has actually established the
        // connection. Handing out a database before that makes every operation
        // block for the connect timeout (5s by default), so a configured-but-
        // unreachable Redis would make the site far slower than no Redis at all.
        // Returning null instead keeps the uncached path instant, and caching
        // switches on by itself once Redis comes up.
        return _multiplexer is { IsConnected: true } multiplexer ? multiplexer.GetDatabase() : null;
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        var configuration = _options.Value.Configuration;
        if (string.IsNullOrWhiteSpace(configuration))
            return;

        try
        {
            // AbortOnConnectFail=false keeps the multiplexer retrying in the background
            // after an outage, so a transient Redis failure degrades to uncached
            // serving instead of throwing for the rest of the process lifetime.
            var options = ConfigurationOptions.Parse(configuration);
            options.AbortOnConnectFail = false;

            // Bound the cost of a Redis that is up but slow, or that drops mid-flight:
            // the IsConnected check above already makes a *down* Redis free.
            options.ConnectTimeout = 2000;
            options.SyncTimeout = 2000;
            options.KeepAlive = 60;

            _multiplexer = await ConnectionMultiplexer.ConnectAsync(options);
            _logger.LogInformation("Output caching connected to Redis ({Configuration}).", configuration);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Redis is unreachable at {Configuration}; public API responses will not be " +
                "cached until Redis is available. The site continues to work uncached. {Message}",
                configuration, ex.Message);
        }
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _multiplexer?.Dispose();
    }
}
