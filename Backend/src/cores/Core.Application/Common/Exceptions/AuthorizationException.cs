using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;

/// <summary>
/// Yetkilendirme (authorization) hatası olduğunda fırlatılan hata.
/// </summary>
public class AuthorizationException : ApplicationException
{
    public AuthorizationException(
        string message,
        string? userFriendlyMessage = null,
        string? errorCode = null,
        object? additionalData = null)
        : base(message, errorCode ?? ApplicationErrorCodes.AuthGeneral, userFriendlyMessage, additionalData)
    {
    }
}