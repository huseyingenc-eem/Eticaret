using Core.Application.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Application.Behaviors.Caching;

/// <summary>
/// MediatR pipeline'ı için önbelleğe ekleme ve okuma işlemlerini yöneten davranış.
/// ICachableRequest arayüzünü uygulayan isteklerin yanıtlarını önbelleğe alır veya önbellekten döner.
/// </summary>
/// <typeparam name="TRequest">İşlenecek MediatR isteği (ICachableRequest olmalı).</typeparam>
/// <typeparam name="TResponse">İsteğin dönüş tipi.</typeparam>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICachableRequest
{
    private readonly IDistributedCache _cache;
    private readonly ISerializerService _serializerService;
    private readonly CacheSettings _cacheSettings;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// CachingBehavior sınıfının bir örneğini oluşturur.
    /// </summary>
    /// <param name="cache">Veri önbellekleme işlemleri için kullanılacak IDistributedCache servisi.</param>
    /// <param name="cacheSettingsOptions">Uygulama genelindeki önbellek ayarlarını içeren yapılandırma.</param>
    /// <param name="serializerService">Nesneleri serileştirme ve deserileştirme işlemleri için servis.</param>
    public CachingBehavior(IDistributedCache cache, IOptions<CacheSettings> cacheSettingsOptions, ISerializerService serializerService)
    {
        _cache = cache;
        _serializerService = serializerService;
        _cacheSettings = cacheSettingsOptions.Value;
    }

    /// <summary>
    /// Gelen isteği işler. Önbellekte veri varsa döner, yoksa isteği çalıştırır ve sonucu önbelleğe ekler.
    /// </summary>
    /// <param name="request">İşlenecek MediatR isteği.</param>
    /// <param name="next">Pipeline'daki bir sonraki adıma geçişi sağlayan delege.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>İsteğin önbellekten veya handler'dan gelen yanıtı.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.BypassCache || string.IsNullOrWhiteSpace(request.CacheKey))
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

        byte[] serializedData = _serializerService.SerializeToUtf8Bytes(response);

        await _cache.SetAsync(request.CacheKey!, serializedData, cacheEntryOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.CacheGroupKey))
        {
            await AddCacheKeyToGroupAsync(request.CacheGroupKey, request.CacheKey!, cacheEntryOptions, cancellationToken);
        }

        return response;
    }

    /// <summary>
    /// Verilen bir önbellek anahtarını, belirtilen grup anahtarı altındaki bir sete ekler.
    /// Bu, belirli bir gruba ait tüm önbellek girişlerini tek seferde geçersiz kılmayı kolaylaştırır.
    /// </summary>
    private async Task AddCacheKeyToGroupAsync(string groupKey, string cacheKey, DistributedCacheEntryOptions itemEntryOptions, CancellationToken cancellationToken)
    {
        byte[]? cachedGroupBytes = await _cache.GetAsync(groupKey, cancellationToken);

        HashSet<string> cacheKeysInGroup = (cachedGroupBytes != null && cachedGroupBytes.Length > 0)
            ? (_serializerService.Deserialize<HashSet<string>>(cachedGroupBytes) ?? new HashSet<string>())
            : new HashSet<string>();

        if (cacheKeysInGroup.Add(cacheKey))
        {
            byte[] newGroupBytes = _serializerService.SerializeToUtf8Bytes(cacheKeysInGroup);
            await _cache.SetAsync(groupKey, newGroupBytes, itemEntryOptions, cancellationToken);
        }
    }
}