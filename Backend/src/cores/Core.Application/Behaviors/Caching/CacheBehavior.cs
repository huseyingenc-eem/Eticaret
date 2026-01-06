using Core.Application.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Core.Application.Behaviors.Caching;

public class CacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICachableRequest
{
    private readonly IDistributedCache _cache;
    private readonly ILoggerService _loggerService;

    public CacheBehavior(IDistributedCache cache, ILoggerService loggerService)
    {
        _cache = cache;
        _loggerService = loggerService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        if (request.BypassCache)
        {
            _loggerService.Info($"🚫 Cache bypassed for request: {requestName}");
            return await next();
        }

        string cacheKey = request.CacheKey;
        if (string.IsNullOrEmpty(cacheKey))
        {
            _loggerService.Info($"📭 No cache key found for request: {requestName}, passing through");
            return await next();
        }

        try
        {
            // Önbellekten okumayı dene
            byte[]? cachedValue = await _cache.GetAsync(cacheKey, cancellationToken);
            if (cachedValue != null)
            {
                _loggerService.Info($"✅ Cache HIT for key: {cacheKey}");
                string cachedJsonString = Encoding.UTF8.GetString(cachedValue);
                TResponse cachedResponse = JsonSerializer.Deserialize<TResponse>(cachedJsonString)!;
                return cachedResponse;
            }

            // Cache Miss - Handler'ı çalıştır
            _loggerService.Info($"❌ Cache MISS for key: {cacheKey}. Getting data from handler.");
            TResponse response = await next();

            // Yanıtı önbelleğe kaydet
            await SetCacheAsync(request, response, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            _loggerService.Error($"💥 Cache operation failed for key: {cacheKey}, Request: {requestName}", ex);
            // Cache hatası uygulamayı durdurmamalı, handler'a devam et
            return await next();
        }
    }

    private async Task SetCacheAsync(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        try
        {
            _loggerService.Info($"💾 Setting cache for key: {request.CacheKey}");

            var options = new DistributedCacheEntryOptions();

            if (request.SlidingExpiration.HasValue)
                options.SetSlidingExpiration(request.SlidingExpiration.Value);

            if (request.AbsoluteExpirationRelativeToNow.HasValue)
                options.SetAbsoluteExpiration(request.AbsoluteExpirationRelativeToNow.Value);

            string jsonString = JsonSerializer.Serialize(response);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);

            await _cache.SetAsync(request.CacheKey!, bytes, options, cancellationToken);

            _loggerService.Info($"✅ Cache SET successfully for key: {request.CacheKey}");

            // Önbellek grubunu güncelle
            if (!string.IsNullOrEmpty(request.CacheGroupKey))
            {
                await UpdateCacheGroupAsync(request.CacheGroupKey, request.CacheKey!, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _loggerService.Error($"💥 Failed to set cache for key: {request.CacheKey}", ex);
            // Cache kaydetme hatası uygulamayı durdurmamalı
        }
    }

    private async Task UpdateCacheGroupAsync(string groupKey, string cacheKey, CancellationToken cancellationToken)
    {
        try
        {
            _loggerService.Info($"🏷️ Adding key '{cacheKey}' to group '{groupKey}'");

            byte[]? cachedGroup = await _cache.GetAsync(groupKey, cancellationToken);
            HashSet<string> keysInGroup;

            if (cachedGroup != null)
            {
                keysInGroup = JsonSerializer.Deserialize<HashSet<string>>(Encoding.UTF8.GetString(cachedGroup)) ?? new HashSet<string>();
                _loggerService.Info($"📂 Group '{groupKey}' found with {keysInGroup.Count} keys");
            }
            else
            {
                keysInGroup = new HashSet<string>();
                _loggerService.Info($"📂 Group '{groupKey}' not found, creating new group");
            }

            // Gruba yeni anahtarı ekle
            bool isKeyAdded = keysInGroup.Add(cacheKey);

            if (!isKeyAdded)
            {
                _loggerService.Info($"⚠️ Key '{cacheKey}' already exists in group '{groupKey}'");
                return;
            }

            // Güncellenmiş grubu kaydet
            string groupJson = JsonSerializer.Serialize(keysInGroup);
            byte[] groupBytes = Encoding.UTF8.GetBytes(groupJson);

            var groupOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(24) // Grupların ömrü uzun olsun
            };

            await _cache.SetAsync(groupKey, groupBytes, groupOptions, cancellationToken);

            _loggerService.Info($"✅ Successfully added key to group. Group '{groupKey}' now contains {keysInGroup.Count} keys: [{string.Join(", ", keysInGroup)}]");
        }
        catch (Exception ex)
        {
            _loggerService.Error($"💥 Failed to update cache group '{groupKey}' with key '{cacheKey}'", ex);
            // Cache group güncelleme hatası uygulamayı durdurmamalı
        }
    }
}