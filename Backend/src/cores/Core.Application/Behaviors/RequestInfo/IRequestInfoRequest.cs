namespace Core.Application.Behaviors.RequestInfo;

/// <summary>
/// Bir MediatR isteğinin (command/query), işlenmeden önce kimliği doğrulanmış
/// bir kullanıcı kimliğine (UserId) ihtiyaç duyduğunu belirten sözleşme.
/// Bu arayüzü uygulayan komutlara, middleware tarafından otomatik olarak UserId atanacaktır.
/// </summary>
public interface IRequestInfoRequest
{
    /// <summary>
    /// İsteği yapan kullanıcının kimliği.
    /// Bu alan, middleware tarafından JWT token'dan okunarak doldurulur.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// İsteğin yaşam döngüsü boyunca takibi için kullanılan benzersiz kimlik.
    /// </summary>
    //public Guid CorrelationId { get; set; }

    /// <summary>
    /// İsteğin dil ve bölge bilgisi (örn: "tr-TR", "en-US").
    /// </summary>
    //public string? Culture { get; set; }
}