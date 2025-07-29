using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;

/// <summary>
/// Kimlik doğrulama (authentication) hatalarında fırlatılan exception.
/// 401 Unauthorized durumları için kullanılır.
/// </summary>
public class AuthenticationException : ApplicationException
{
    public AuthenticationException(
        string message = "Kimlik doğrulama başarısız.",
        string? userFriendlyMessage = "Oturum açmanız gerekiyor. Lütfen giriş yapınız.",
        string? errorCode = null)
        : base(message, errorCode ?? ApplicationErrorCodes.AuthGeneral, userFriendlyMessage)
    {
    }
}