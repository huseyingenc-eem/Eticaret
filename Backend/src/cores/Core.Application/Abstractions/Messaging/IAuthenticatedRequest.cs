namespace Core.Application.Abstractions.Messaging;

/// <summary>
/// Bir MediatR isteğinin (command/query), işlenmeden önce kimliği doğrulanmış
/// bir kullanıcı kimliğine (UserId) ihtiyaç duyduğunu belirten sözleşme.
/// Bu arayüzü uygulayan komutlara, middleware tarafından otomatik olarak UserId atanacaktır.
/// </summary>
public interface IAuthenticatedRequest
{
    /// <summary>
    /// İsteği yapan kullanıcının kimliği.
    /// Bu alan, middleware tarafından JWT token'dan okunarak doldurulur.
    /// </summary>
    public string UserId { get; set; }
}