namespace Core.Application.Contracts.Responses;

/// <summary>
/// API'den dönen tüm standart yanıtlar için temel zarf (envelope) yapısı.
/// </summary>
/// <typeparam name="T">Yanıtın içinde taşınacak olan asıl verinin (payload) türü.</typeparam>
public class BaseResponse<T>
{
    /// <summary>
    /// İsteğin başarılı olup olmadığını gösterir.
    /// Her zaman `true` olacaktır, çünkü hatalar global exception middleware tarafından yakalanır.
    /// </summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// Yanıtın içinde taşınan asıl veri.
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// İsteği başlatan CorrelationId'yi geri döndürerek,
    /// istemcinin isteği ve yanıtı eşleştirmesine olanak tanır.
    /// </summary>
    public Guid CorrelationId { get; set; }

    public BaseResponse(T data, Guid correlationId)
    {
        Data = data;
        CorrelationId = correlationId;
    }
}