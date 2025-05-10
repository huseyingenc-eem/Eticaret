namespace Core.CrossCuttingConcerns.Exceptions;

public class AuthorizationException : CoreException
{
    public AuthorizationException(string errorCode, string message, string? userFriendlyMessage = null, object? additionalData = null, Exception? innerException = null)
        : base(errorCode, message, userFriendlyMessage, additionalData, innerException)
    {
    }

    public AuthorizationException(string message)
        : base(Constants.ErrorCodes.AuthGeneral, message, message, null, null)
    {
    }
}
