using Microsoft.Extensions.Caching.Distributed;

namespace Core.Application.Abstractions.Services;

/// <summary>
/// Uygulama genelinde önbellekleme işlemlerini yöneten servis sözleşmesi.
/// Bu arayüz teknolojiden bağımsızdır.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Veriyi cache'e ekler.
    /// </summary>
    /// <typeparam name="T">Eklenecek verinin türü.</typeparam>
    /// <param name="key">Cache anahtarı.</param>
    /// <param name="value">Eklenecek veri.</param>
    /// <param name="options">Cache giriş ayarları (ömrü vb.).</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    Task AddDataAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cache'den veri çeker.
    /// </summary>
    /// <typeparam name="T">Çekilecek verinin türü.</typeparam>
    /// <param name="key">Cache anahtarı.</param>
    /// <returns>Cache'deki veri veya bulunamazsa default değer.</returns>
    Task<T?> GetDataAsync<T>(string key);

    /// <summary>
    /// Cache'den veri siler.
    /// </summary>
    /// <param name="key">Silinecek verinin cache anahtarı.</param>
    Task RemoveDataAsync(string key);
}