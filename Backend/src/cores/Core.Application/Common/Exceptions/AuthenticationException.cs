namespace Core.Application.Common.Exceptions;

/// <summary>
/// Kimlik doğrulama (authentication) hatalarında fırlatılan exception.
/// 401 Unauthorized durumları için kullanılır.
/// </summary>
public class AuthenticationException : ApplicationException
{
    public AuthenticationException(string message = "Kimlik doğrulama başarısız.")
        : base(
            message: message,
            userFriendlyMessage: "Oturum açmanız gerekiyor. Lütfen giriş yapınız.",
            errorCode: "AUTHENTICATION_REQUIRED"
        )
    {
    }

    public AuthenticationException(string message, string? userFriendlyMessage, string? errorCode = null)
        : base(
            message: message,
            userFriendlyMessage: userFriendlyMessage,
            errorCode: errorCode ?? "AUTHENTICATION_REQUIRED"
        )
    {
    }
}