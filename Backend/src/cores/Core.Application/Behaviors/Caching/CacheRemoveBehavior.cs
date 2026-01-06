using Core.Application.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Core.Application.Behaviors.Caching;

public class CacheRemoveBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>, ICacheRemoverRequest
{
    private readonly IDistributedCache _cache;
    private readonly ILoggerService _loggerService;

    public CacheRemoveBehavior(IDistributedCache cache, ILoggerService loggerService)
    {
        _cache = cache;
        _loggerService = loggerService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        if (request.BypassCache)
        {
            _loggerService.Info($"🚫 Cache removal bypassed for request: {requestName}");
            return await next();
        }

        _loggerService.Info($"🔥 Starting cache removal for request: {requestName}");
        _loggerService.Info($"🔑 CacheGroupKey: {request.CacheGroupKey ?? "null"}, CacheKey: {request.CacheKey ?? "null"}");

        // Handler'ı önce çalıştır
        TResponse response = await next();

        try
        {
            // ✅ Remove cache group first
            if (!string.IsNullOrEmpty(request.CacheGroupKey))
            {
                _loggerService.Info($"🗂️ Attempting to remove cache group: {request.CacheGroupKey}");
                await RemoveCacheGroupAsync(request.CacheGroupKey, cancellationToken);
            }

            // ✅ Then remove specific cache key
            if (!string.IsNullOrEmpty(request.CacheKey))
            {
                await _cache.RemoveAsync(request.CacheKey, cancellationToken);
                _loggerService.Info($"✅ Single cache key removed: {request.CacheKey}");
            }

            _loggerService.Info($"🎉 Cache removal completed successfully for request: {requestName}");
        }
        catch (Exception ex)
        {
            _loggerService.Error($"💥 Cache removal failed for request: {requestName}", ex);
            // Cache hatası uygulamayı durdurmamalı
        }

        return response;
    }

    private async Task RemoveCacheGroupAsync(string groupKey, CancellationToken cancellationToken)
    {
        try
        {
            _loggerService.Info($"🔍 Looking for cache group: {groupKey}");

            byte[]? cachedGroup = await _cache.GetAsync(groupKey, cancellationToken);
            if (cachedGroup != null)
            {
                HashSet<string>? keysInGroup = JsonSerializer
                    .Deserialize<HashSet<string>>(Encoding.UTF8.GetString(cachedGroup));

                if (keysInGroup != null && keysInGroup.Count > 0)
                {
                    _loggerService.Info($"📦 Found cache group '{groupKey}' with {keysInGroup.Count} keys: [{string.Join(", ", keysInGroup)}]");

                    // Remove all keys in the group
                    int removedCount = 0;
                    foreach (var key in keysInGroup)
                    {
                        await _cache.RemoveAsync(key, cancellationToken);
                        removedCount++;
                        _loggerService.Info($"🗑️ Cache key removed from group: {key}");
                    }

                    _loggerService.Info($"✅ Successfully removed {removedCount} cache entries from group: {groupKey}");
                }
                else
                {
                    _loggerService.Warning($"⚠️ Cache group '{groupKey}' exists but contains no keys");
                }

                // Remove the group itself
                await _cache.RemoveAsync(groupKey, cancellationToken);
                _loggerService.Info($"🗂️ Cache group removed: {groupKey}");
            }
            else
            {
                _loggerService.Warning($"❌ Cache group not found: {groupKey}");
            }
        }
        catch (Exception ex)
        {
            _loggerService.Error($"💥 Error removing cache group: {groupKey}", ex);
            // Don't rethrow - cache errors shouldn't break the application
        }
    }
}