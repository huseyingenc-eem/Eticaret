using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;

public class AuthorizationException : ApplicationException
{
    public AuthorizationException(string errorCode, string message, string? userFriendlyMessage = null, object? additionalData = null, Exception? innerException = null)
        : base(errorCode, message, userFriendlyMessage, additionalData, innerException)
    {
    }

    public AuthorizationException(string message)
        : base(ApplicationErrorCodes.AuthGeneral, message, message, null, null)
    {
    }
}
