using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;

/// <summary>
/// İstenen bir kaynak bulunamadığında fırlatılan hata.
/// </summary>
public class NotFoundException : ApplicationException
{
    /// <summary>
    /// NotFoundException sınıfının yeni bir örneğini oluşturur.
    /// Gerekli tüm bilgileri ApplicationException temel sınıfına iletir.
    /// </summary>
    /// <param name="message">Geliştirici odaklı, teknik hata mesajı.</param>
    /// <param name="userFriendlyMessage">Son kullanıcıya gösterilebilecek anlaşılır mesaj (opsiyonel).</param>
    /// <param name="errorCode">Hata kodu (varsayılan olarak NotFoundGeneral kullanılır).</param>
    public NotFoundException(
        string message,
        string? userFriendlyMessage = null,
        string? errorCode = ApplicationErrorCodes.NotFoundGeneral)
        : base(message, userFriendlyMessage, errorCode)
    {
    }
}