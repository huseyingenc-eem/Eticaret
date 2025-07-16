using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using Core.Application.Abstractions.Services;

namespace Core.Infrastructure.Services.Caching;

/// <summary>
/// Redis gibi dağıtık bir önbellek mekanizması kullanarak veri ekleme, alma ve silme işlemlerini yöneten somut servis sınıfı.
/// Bu sınıf, ICacheService arayüzünü implemente ederek, uygulamanın önbellekleme altyapısıyla konuşmasını sağlar.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILoggerService _loggerService;

    /// <summary>
    /// RedisCacheService sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="distributedCache">Microsoft'un dağıtık önbellek altyapısı için temel arayüz.</param>
    /// <param name="loggerService">Uygulama genelinde loglama işlemleri için kullanılan servis.</param>
    public RedisCacheService(IDistributedCache distributedCache, ILoggerService loggerService)
    {
        _distributedCache = distributedCache;
        _loggerService = loggerService;
    }

    /// <summary>
    /// Belirtilen anahtar (key) ile veriyi asenkron olarak önbelleğe ekler.
    /// </summary>
    /// <typeparam name="T">Önbelleğe eklenecek verinin türü.</typeparam>
    /// <param name="key">Veri için benzersiz önbellek anahtarı.</param>
    /// <param name="value">Önbelleğe eklenecek olan veri.</param>
    /// <param name="options">Önbellek giriş ayarları (örn: son kullanma süresi).</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    public async Task AddDataAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            // Veriyi JSON formatına serileştir
            var jsonData = JsonSerializer.Serialize(value);
            // JSON string'ini byte dizisine çevir
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

            // Veriyi önbelleğe ekle
            await _distributedCache.SetAsync(key, dataBytes, options, cancellationToken);
        }
        catch (Exception ex)
        {
            // Hata durumunda loglama yap ve istisnayı yeniden fırlat
            _loggerService.Error($"RedisCacheService AddDataAsync metodu {key} anahtarı için hata verdi: {ex.Message}", ex);
            throw new Exception($"RedisCacheService AddDataAsync hatası. Detaylar için iç istisnaya bakın. Anahtar: {key}", ex);
        }
    }

    /// <summary>
    /// Belirtilen anahtar (key) ile önbellekten veriyi asenkron olarak alır.
    /// </summary>
    /// <typeparam name="T">Önbellekten alınacak verinin türü.</typeparam>
    /// <param name="key">Verinin benzersiz önbellek anahtarı.</param>
    /// <returns>Önbellekte bulunan veri. Eğer veri bulunamazsa, T tipi için varsayılan değeri döner.</returns>
    public async Task<T> GetDataAsync<T>(string key)
    {
        try
        {
            // Anahtara karşılık gelen veriyi byte dizisi olarak al
            byte[] datas = await _distributedCache.GetAsync(key);

            // Veri bulunamadıysa varsayılan değeri dön
            if (datas == null)
                return default;

            // Byte dizisini tekrar JSON string'ine çevir
            var jsonData = Encoding.UTF8.GetString(datas);

            // JSON string'ini belirtilen T tipine dönüştür
            T response = JsonSerializer.Deserialize<T>(jsonData);

            return response;
        }
        catch (JsonException jsonEx)
        {
            _loggerService.Error($"RedisCacheService GetDataAsync metodu {key} anahtarı için JSON dönüştürme hatası: {jsonEx.Message}", jsonEx);

            // Opsiyonel: Önbellekteki bozuk veri, bir sonraki okumada tekrar hataya neden olmasın diye silinebilir.
            await RemoveDataAsync(key);

            throw new Exception($"RedisCacheService GetDataAsync hatası: {key} anahtarı için veri dönüştürülemedi. Detaylar için iç istisnaya bakın.", jsonEx);
        }
        catch (Exception ex)
        {
            _loggerService.Error($"RedisCacheService GetDataAsync metodu {key} anahtarı için hata verdi: {ex.Message}", ex);
            throw new Exception($"RedisCacheService GetDataAsync hatası. Detaylar için iç istisnaya bakın. Anahtar: {key}", ex);
        }
    }

    /// <summary>
    /// Belirtilen anahtar (key) ile önbellekteki veriyi asenkron olarak siler.
    /// </summary>
    /// <param name="key">Silinecek verinin benzersiz önbellek anahtarı.</param>
    public async Task RemoveDataAsync(string key)
    {
        try
        {
            await _distributedCache.RemoveAsync(key);
            _loggerService.Info($"{key} anahtarına sahip veri önbellekten silindi.");
        }
        catch (Exception ex)
        {
            _loggerService.Error($"RedisCacheService RemoveDataAsync metodu {key} anahtarı için hata verdi: {ex.Message}", ex);
            throw new Exception($"RedisCacheService RemoveDataAsync hatası. Detaylar için iç istisnaya bakın. Anahtar: {key}", ex);
        }
    }
}