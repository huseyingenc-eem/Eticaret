using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options; // IOptions için
using System; // TimeSpan, ArgumentNullException için
using System.Collections.Generic; // HashSet için
using System.Linq; // FirstOrDefault için (kullanılmıyor ama genel using)
using System.Text; // Encoding için
using System.Text.Json; // JsonSerializer için
using System.Threading; // CancellationToken için
using System.Threading.Tasks; // Task için

namespace Core.Application.Pipelines.Caching;

/// <summary>
/// MediatR pipeline'ında önbelleğe ekleme işlemlerini gerçekleştiren davranış (behavior).
/// ICachableRequest arayüzünü implemente eden isteklerin yanıtlarını önbelleğe alır.
/// </summary>
/// <typeparam name="TRequest">İşlenecek MediatR isteğinin tipi.</typeparam>
/// <typeparam name="TResponse">MediatR isteğinin dönüş tipi.</typeparam>
public class AddCachePipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICachableRequest
{
    private readonly CacheSettings _cacheSettings;
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// AddCachePipeline sınıfının bir örneğini başlatır.
    /// </summary>
    /// <param name="cache">Kullanılacak IDistributedCache örneği.</param>
    /// <param name="cacheSettingsOptions">Önbellek ayarlarını içeren IOptions örneği.</param>
    public AddCachePipeline(IDistributedCache cache, IOptions<CacheSettings> cacheSettingsOptions)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheSettings = cacheSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(cacheSettingsOptions));
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// MediatR isteğini işler. Yanıtı önbellekten alır veya kaynağa gidip sonucu önbelleğe ekler.
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.ByPassCache || string.IsNullOrWhiteSpace(request.CacheKey))
        {
            return await next();
        }
        ArgumentNullException.ThrowIfNullOrWhiteSpace(request.CacheKey);

        TResponse? response;
        byte[]? cachedResponseBytes = await _cache.GetAsync(request.CacheKey, cancellationToken);

        if (cachedResponseBytes != null && cachedResponseBytes.Length > 0)
        {
            
            response = JsonSerializer.Deserialize<TResponse>(Encoding.UTF8.GetString(cachedResponseBytes), _jsonSerializerOptions);
            if (response == null)
               
                await _cache.RemoveAsync(request.CacheKey, cancellationToken);
                response = await GetResponseAndAddToCache(request, next, cancellationToken);
        }
        else
            response = await GetResponseAndAddToCache(request, next, cancellationToken);

        return response!; 
    }

    /// <summary>
    /// Kaynaktan yanıtı alır ve belirlenen ayarlara göre önbelleğe ekler.
    /// </summary>
    private async Task<TResponse> GetResponseAndAddToCache(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response = await next();

       
        var cacheEntryOptions = new DistributedCacheEntryOptions();

        TimeSpan slidingExpiration = request.SlidingExpiration
                                     ?? TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationInMinutes);
        if (slidingExpiration > TimeSpan.Zero)
        {
            cacheEntryOptions.SlidingExpiration = slidingExpiration;
        }
        TimeSpan? absoluteExpirationRelativeToNow = request.AbsoluteExpirationRelativeToNow;
        if (!absoluteExpirationRelativeToNow.HasValue && _cacheSettings.AbsoluteExpirationInMinutes.HasValue && _cacheSettings.AbsoluteExpirationInMinutes.Value > 0)
        {
            absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheSettings.AbsoluteExpirationInMinutes.Value);
        }

        if (absoluteExpirationRelativeToNow.HasValue && absoluteExpirationRelativeToNow.Value > TimeSpan.Zero)
        {
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        }

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
    /// Grup anahtarının kendisi de uygun bir son kullanma süresiyle önbelleğe alınır.
    /// </summary>
    private async Task AddCacheKeyToGroupAsync(string groupKey, string cacheKey, DistributedCacheEntryOptions itemEntryOptions, CancellationToken cancellationToken)
    {
        byte[]? cachedGroupBytes = await _cache.GetAsync(groupKey, cancellationToken);
        HashSet<string> cacheKeysInGroup;

        if (cachedGroupBytes != null && cachedGroupBytes.Length > 0)
        {
            cacheKeysInGroup = JsonSerializer.Deserialize<HashSet<string>>(Encoding.UTF8.GetString(cachedGroupBytes), _jsonSerializerOptions) ?? new HashSet<string>();
        }
        else
        {
            cacheKeysInGroup = new HashSet<string>();
        }

        if (cacheKeysInGroup.Add(cacheKey)) 
        {
            byte[] newGroupBytes = JsonSerializer.SerializeToUtf8Bytes(cacheKeysInGroup, _jsonSerializerOptions);

            
           
            var groupCacheEntryOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = itemEntryOptions.SlidingExpiration,
                AbsoluteExpirationRelativeToNow = itemEntryOptions.AbsoluteExpirationRelativeToNow
            };
            await _cache.SetAsync(groupKey, newGroupBytes, groupCacheEntryOptions, cancellationToken);
        }
    }
}
