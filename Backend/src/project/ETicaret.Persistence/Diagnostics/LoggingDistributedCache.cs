using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ETicaret.Persistence.Diagnostics;

// Cache işlemlerini loglayan decorator
public class LoggingDistributedCache : IDistributedCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<LoggingDistributedCache> _logger;
    private readonly CacheLoggingOptions _options;
    private static int _cacheCounter = 0;

    public LoggingDistributedCache(
        IDistributedCache cache,
        ILogger<LoggingDistributedCache> logger,
        CacheLoggingOptions options)
    {
        _cache = cache;
        _logger = logger;
        _options = options;

        Console.WriteLine(Colorize("🔄 Redis Logging Cache initialized!", ConsoleColor.Blue));
    }

    public byte[]? Get(string key)
    {
        var queryId = Interlocked.Increment(ref _cacheCounter);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = _cache.Get(key);
            stopwatch.Stop();

            LogCacheOperation("GET", key, queryId, stopwatch.ElapsedMilliseconds,
                result != null, result?.Length ?? 0);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogCacheError("GET", key, queryId, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        var queryId = Interlocked.Increment(ref _cacheCounter);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await _cache.GetAsync(key, token);
            stopwatch.Stop();

            LogCacheOperation("GET_ASYNC", key, queryId, stopwatch.ElapsedMilliseconds,
                result != null, result?.Length ?? 0);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogCacheError("GET_ASYNC", key, queryId, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        var queryId = Interlocked.Increment(ref _cacheCounter);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _cache.Set(key, value, options);
            stopwatch.Stop();

            LogCacheOperation("SET", key, queryId, stopwatch.ElapsedMilliseconds,
                true, value.Length, options);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogCacheError("SET", key, queryId, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }

    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        var queryId = Interlocked.Increment(ref _cacheCounter);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await _cache.SetAsync(key, value, options, token);
            stopwatch.Stop();

            LogCacheOperation("SET_ASYNC", key, queryId, stopwatch.ElapsedMilliseconds,
                true, value.Length, options);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogCacheError("SET_ASYNC", key, queryId, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }

    public void Refresh(string key)
    {
        var queryId = Interlocked.Increment(ref _cacheCounter);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _cache.Refresh(key);
            stopwatch.Stop();

            LogCacheOperation("REFRESH", key, queryId, stopwatch.ElapsedMilliseconds, true, 0);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogCacheError("REFRESH", key, queryId, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        var queryId = Interlocked.Increment(ref _cacheCounter);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await _cache.RefreshAsync(key, token);
            stopwatch.Stop();

            LogCacheOperation("REFRESH_ASYNC", key, queryId, stopwatch.ElapsedMilliseconds, true, 0);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogCacheError("REFRESH_ASYNC", key, queryId, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }

    public void Remove(string key)
    {
        var queryId = Interlocked.Increment(ref _cacheCounter);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _cache.Remove(key);
            stopwatch.Stop();

            LogCacheOperation("REMOVE", key, queryId, stopwatch.ElapsedMilliseconds, true, 0);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogCacheError("REMOVE", key, queryId, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        var queryId = Interlocked.Increment(ref _cacheCounter);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await _cache.RemoveAsync(key, token);
            stopwatch.Stop();

            LogCacheOperation("REMOVE_ASYNC", key, queryId, stopwatch.ElapsedMilliseconds, true, 0);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogCacheError("REMOVE_ASYNC", key, queryId, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }

    private void LogCacheOperation(string operation, string key, int queryId, long ms,
        bool success, int dataSize, DistributedCacheEntryOptions? options = null)
    {
        var isHit = operation.StartsWith("GET") && success;
        var isMiss = operation.StartsWith("GET") && !success;

        // Performance indicators
        var perfIcon = operation switch
        {
            var op when op.StartsWith("GET") && isHit => "🎯", // Cache Hit
            var op when op.StartsWith("GET") && isMiss => "❌", // Cache Miss
            var op when op.StartsWith("SET") => "💾", // Cache Set
            var op when op.StartsWith("REMOVE") => "🗑️", // Cache Remove
            var op when op.StartsWith("REFRESH") => "🔄", // Cache Refresh
            _ => "📦"
        };

        var title = operation switch
        {
            var op when op.StartsWith("GET") && isHit => "🎯 CACHE HIT",
            var op when op.StartsWith("GET") && isMiss => "❌ CACHE MISS",
            var op when op.StartsWith("SET") => "💾 CACHE SET",
            var op when op.StartsWith("REMOVE") => "🗑️ CACHE REMOVE",
            var op when op.StartsWith("REFRESH") => "🔄 CACHE REFRESH",
            _ => "📦 CACHE OP"
        };

        var titleColor = operation switch
        {
            var op when op.StartsWith("GET") && isHit => ConsoleColor.Green,
            var op when op.StartsWith("GET") && isMiss => ConsoleColor.Yellow,
            var op when op.StartsWith("SET") => ConsoleColor.Blue,
            var op when op.StartsWith("REMOVE") => ConsoleColor.Red,
            _ => ConsoleColor.Cyan
        };

        var sb = new StringBuilder();
        sb.AppendLine(CacheBoxTop());
        sb.AppendLine(CacheBoxLine($"{title} [{queryId:D4}] {perfIcon} {ms:F1}ms - {operation}"));
        sb.AppendLine(CacheBoxSep());
        sb.AppendLine(CacheBoxLine($"🔑 Key: {ShortenKey(key)}"));

        if (dataSize > 0)
            sb.AppendLine(CacheBoxLine($"📊 Data Size: {FormatBytes(dataSize)}"));

        if (options != null)
        {
            var expiry = options.AbsoluteExpirationRelativeToNow?.TotalMinutes ??
                        options.SlidingExpiration?.TotalMinutes;
            if (expiry.HasValue)
                sb.AppendLine(CacheBoxLine($"⏰ TTL: {expiry:F1} minutes"));
        }

        if (operation.StartsWith("GET") && isMiss)
            sb.AppendLine(CacheBoxLine("💡 Next operation will likely hit database"));

        sb.Append(CacheBoxBottom());

        var logMessage = sb.ToString();
        var colorizedMessage = _options.EnableColors ?
            Colorize(logMessage, titleColor) : logMessage;

        var logLevel = isMiss ? LogLevel.Warning : LogLevel.Information;

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CacheQueryId"] = queryId,
            ["CacheOperation"] = operation,
            ["CacheKey"] = key,
            ["ExecutionTimeMs"] = ms,
            ["IsHit"] = isHit,
            ["IsMiss"] = isMiss,
            ["DataSize"] = dataSize
        });

        _logger.Log(logLevel, "Cache Operation\n{CacheBlock}", colorizedMessage);

        if (_options.EchoToConsole)
        {
            Console.WriteLine(colorizedMessage);
            Console.WriteLine();
        }
    }

    private void LogCacheError(string operation, string key, int queryId, long ms, Exception ex)
    {
        var sb = new StringBuilder();
        sb.AppendLine(CacheBoxTop());
        sb.AppendLine(CacheBoxLine($"💥 CACHE ERROR [{queryId:D4}] {ms:F1}ms - {operation}"));
        sb.AppendLine(CacheBoxSep());
        sb.AppendLine(CacheBoxLine($"🔑 Key: {ShortenKey(key)}"));
        sb.AppendLine(CacheBoxLine($"❌ Error: {ex.Message}"));
        sb.Append(CacheBoxBottom());

        var logMessage = sb.ToString();
        var colorizedMessage = _options.EnableColors ?
            Colorize(logMessage, ConsoleColor.Red) : logMessage;

        _logger.LogError(ex, "Cache Operation Failed\n{CacheBlock}", colorizedMessage);

        if (_options.EchoToConsole)
        {
            Console.WriteLine(colorizedMessage);
            Console.WriteLine();
        }
    }

    // Helper methods
    private static string ShortenKey(string key) =>
        key.Length > 60 ? key[..57] + "..." : key;

    private static string FormatBytes(int bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _ => $"{bytes / (1024.0 * 1024.0):F1} MB"
    };

    private static string Colorize(string text, ConsoleColor color)
    {
        var colorCode = color switch
        {
            ConsoleColor.Red => "\x1b[31m",
            ConsoleColor.Green => "\x1b[32m",
            ConsoleColor.Yellow => "\x1b[33m",
            ConsoleColor.Blue => "\x1b[34m",
            ConsoleColor.Magenta => "\x1b[35m",
            ConsoleColor.Cyan => "\x1b[36m",
            ConsoleColor.Gray => "\x1b[90m",
            _ => "\x1b[37m"
        };

        return $"{colorCode}{text}\x1b[0m";
    }

    // Cache box drawing
    private static string CacheBoxTop() => "╔" + new string('═', 75) + "╗";
    private static string CacheBoxSep() => "╠" + new string('═', 75) + "╣";
    private static string CacheBoxBottom() => "╚" + new string('═', 75) + "╝";
    private static string CacheBoxLine(string content)
    {
        const int maxWidth = 73;
        if (content.Length > maxWidth) content = content[..maxWidth];
        return "║ " + content.PadRight(maxWidth) + " ║";
    }
}

// Cache logging options
public class CacheLoggingOptions
{
    public bool EnableColors { get; set; } = true;
    public bool EchoToConsole { get; set; } = true;
    public bool LogCacheHits { get; set; } = true;
    public bool LogCacheMisses { get; set; } = true;
    public bool LogCacheSets { get; set; } = true;
    public int MaxKeyLength { get; set; } = 60;
}

// Repository base class'ınıza cache miss durumunda bilgi verecek extension
public static class CacheAwareRepositoryExtensions
{
    private static readonly ILogger _logger =
        LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger("CacheAwareRepository");

    public static async Task<T?> GetWithCacheLogging<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T?>> databaseFallback,
        TimeSpan? expiration = null) where T : class
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Cache'den veri al
        var cachedData = await cache.GetStringAsync(key);
        stopwatch.Stop();

        if (cachedData != null)
        {
            // Cache hit
            var result = JsonSerializer.Deserialize<T>(cachedData);
            Console.WriteLine(Colorize($"🎯 Cache HIT for key: {key} ({stopwatch.ElapsedMilliseconds}ms)", ConsoleColor.Green));
            return result;
        }

        // Cache miss - database'e git
        Console.WriteLine(Colorize($"❌ Cache MISS for key: {key} - Querying database...", ConsoleColor.Yellow));

        var dbStopwatch = System.Diagnostics.Stopwatch.StartNew();
        var dbResult = await databaseFallback();
        dbStopwatch.Stop();

        if (dbResult != null)
        {
            // Cache'e kaydet
            var serializedData = JsonSerializer.Serialize(dbResult);
            var cacheOptions = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
                cacheOptions.SetAbsoluteExpiration(expiration.Value);

            await cache.SetStringAsync(key, serializedData, cacheOptions);
            Console.WriteLine(Colorize($"💾 Data cached for key: {key} (DB: {dbStopwatch.ElapsedMilliseconds}ms)", ConsoleColor.Blue));
        }

        return dbResult;
    }

    private static string Colorize(string text, ConsoleColor color)
    {
        var colorCode = color switch
        {
            ConsoleColor.Red => "\x1b[31m",
            ConsoleColor.Green => "\x1b[32m",
            ConsoleColor.Yellow => "\x1b[33m",
            ConsoleColor.Blue => "\x1b[34m",
            ConsoleColor.Cyan => "\x1b[36m",
            _ => "\x1b[37m"
        };

        return $"{colorCode}{text}\x1b[0m";
    }
}