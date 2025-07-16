using Core.Application.Abstractions.Services;
using Core.Application.Interfaces.Paging;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Application.Behaviors.Caching;

/// <summary>
/// MediatR pipeline'ı için önbelleğe ekleme ve önbellekten okuma işlemlerini yöneten davranış (behavior).
/// ICachableRequest arayüzünü uygulayan isteklerin yanıtlarını önbelleğe alır veya önbellekten döner.
/// </summary>
/// <typeparam name="TRequest">İşlenecek MediatR isteği (ICachableRequest olmalı).</typeparam>
/// <typeparam name="TResponse">İsteğin dönüş tipi.</typeparam>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICachableRequest
{
    private readonly CacheSettings _cacheSettings;
    
    private readonly JsonSerializerOptions _jsonSerializerOptions;


    private readonly IDistributedCache _cache;
    private readonly ISerializerService _serializerService;


    /// <summary>
    /// AddCachePipeline sınıfının bir örneğini oluşturur.
    /// </summary>
    /// <param name="cache">Veri önbellekleme işlemleri için kullanılacak IDistributedCache servisi.</param>
    /// <param name="cacheSettingsOptions">Uygulama genelindeki önbellek ayarlarını içeren yapılandırma.</param>
    /// <param name="serializerService">Nesneleri JSON'a çevirme ve JSON'dan nesneye dönüştürme işlemleri için servis.</param>
    public CachingBehavior(IDistributedCache cache, IOptions<CacheSettings> cacheSettingsOptions, ISerializerService serializerService)
    {
        _cache = cache;
        _cacheSettings = cacheSettingsOptions.Value;
        _serializerService = serializerService;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// Gelen isteği işler. Önbellekte veri varsa döner, yoksa isteği çalıştırır ve sonucu önbelleğe ekler.
    /// </summary>
    /// <param name="request">İşlenecek MediatR isteği.</param>
    /// <param name="next">Pipeline'daki bir sonraki adıma geçişi sağlayan delege.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>İsteğin yanıtı.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.ByPassCache || string.IsNullOrWhiteSpace(request.CacheKey))
        {
            return await next();
        }

        byte[]? cachedResponseBytes = await _cache.GetAsync(request.CacheKey, cancellationToken);
        if (cachedResponseBytes != null && cachedResponseBytes.Length > 0)
        {
            var response = _serializerService.Deserialize<TResponse>(cachedResponseBytes);
            if (response != null)
            {
                return response;
            }
        }

        return await GetResponseAndAddToCache(request, next, cancellationToken);
    }

    /// <summary>
    /// Pipeline'daki bir sonraki adımı çalıştırarak yanıtı alır ve sonucu belirlenen ayarlara göre önbelleğe ekler.
    /// </summary>
    private async Task<TResponse> GetResponseAndAddToCache(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response = await next();

        TimeSpan slidingExpiration = request.SlidingExpiration ?? TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationInMinutes);
        TimeSpan? absoluteExpiration = request.AbsoluteExpirationRelativeToNow ??
                                       (_cacheSettings.AbsoluteExpirationInMinutes.HasValue ? TimeSpan.FromMinutes(_cacheSettings.AbsoluteExpirationInMinutes.Value) : null);

        var cacheEntryOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = slidingExpiration > TimeSpan.Zero ? slidingExpiration : null,
            AbsoluteExpirationRelativeToNow = absoluteExpiration > TimeSpan.Zero ? absoluteExpiration : null
        };

        byte[] serializedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response, _jsonSerializerOptions));

        await _cache.SetAsync(request.CacheKey!, serializedData, cacheEntryOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.CacheGroupKey))
        {
            await AddCacheKeyToGroupAsync(request.CacheGroupKey, request.CacheKey!, cacheEntryOptions, cancellationToken);
        }

        return response;
    }

    /// <summary>
    /// Verilen bir önbellek anahtarını, belirtilen grup anahtarı altındaki bir sete ekler.
    /// </summary>
    private async Task AddCacheKeyToGroupAsync(string groupKey, string cacheKey, DistributedCacheEntryOptions itemEntryOptions, CancellationToken cancellationToken)
    {
        byte[]? cachedGroupBytes = await _cache.GetAsync(groupKey, cancellationToken);

        HashSet<string> cacheKeysInGroup = (cachedGroupBytes != null && cachedGroupBytes.Length > 0)
            ? (JsonSerializer.Deserialize<HashSet<string>>(Encoding.UTF8.GetString(cachedGroupBytes), _jsonSerializerOptions) ?? new HashSet<string>())
            : new HashSet<string>();

        if (cacheKeysInGroup.Add(cacheKey))
        {
            byte[] newGroupBytes = JsonSerializer.SerializeToUtf8Bytes(cacheKeysInGroup, _jsonSerializerOptions);
            await _cache.SetAsync(groupKey, newGroupBytes, itemEntryOptions, cancellationToken);
        }
    }
}