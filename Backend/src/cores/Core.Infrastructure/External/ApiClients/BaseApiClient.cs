// Konum: cores/Core.Infrastructure/External/ApiClients/BaseApiClient.cs
using Core.Application.Abstractions.Services;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Core.Infrastructure.External.ApiClients;

/// <summary>
/// Üçüncü parti REST API'lerle iletişim kurmak için ortak işlevsellik sağlayan temel sınıf.
/// HttpClient yönetimini, serileştirme ayarlarını ve standart GET/POST isteklerini merkezileştirir.
/// </summary>
public abstract class BaseApiClient
{
    protected readonly HttpClient HttpClient;
    protected readonly ISerializerService SerializerService;

    /// <summary>
    /// BaseApiClient'in yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="httpClient">HTTP isteklerini yapmak için kullanılacak HttpClient nesnesi.</param>
    /// <param name="serializerService">İstek ve yanıt gövdelerini serileştirmek/deserileştirmek için kullanılır.</param>
    protected BaseApiClient(HttpClient httpClient, ISerializerService serializerService)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        SerializerService = serializerService ?? throw new ArgumentNullException(nameof(serializerService));

        // Varsayılan olarak JSON content type kabul ettiğimizi belirtelim.
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Belirtilen URI'ye bir GET isteği gönderir ve yanıtı TResponse tipine dönüştürür.
    /// </summary>
    /// <typeparam name="TResponse">Yanıtın dönüştürüleceği tip.</typeparam>
    /// <param name="requestUri">İsteğin gönderileceği URI.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>API'den dönen ve TResponse tipine dönüştürülmüş veri.</returns>
    protected virtual async Task<TResponse?> GetAsync<TResponse>(string requestUri, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode(); // HTTP 2xx dışında bir kod dönerse hata fırlatır.

        var responseData = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        return SerializerService.Deserialize<TResponse>(responseData);
    }

    /// <summary>
    /// Belirtilen URI'ye bir POST isteği gönderir ve yanıtı TResponse tipine dönüştürür.
    /// </summary>
    /// <typeparam name="TRequest">İstek gövdesinin tipi.</typeparam>
    /// <typeparam name="TResponse">Yanıtın dönüştürüleceği tip.</typeparam>
    /// <param name="requestUri">İsteğin gönderileceği URI.</param>
    /// <param name="requestData">POST isteğinin gövdesinde gönderilecek veri.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>API'den dönen ve TResponse tipine dönüştürülmüş veri.</returns>
    protected virtual async Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUri, TRequest requestData, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync(requestUri, requestData, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        return SerializerService.Deserialize<TResponse>(responseData);
    }
}