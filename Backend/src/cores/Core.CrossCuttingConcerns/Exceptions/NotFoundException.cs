using Core.CrossCuttingConcerns.Constants;

namespace Core.CrossCuttingConcerns.Exceptions;

public class NotFoundException : CoreException
{
    public NotFoundException(string errorCode, string message, string? userFriendlyMessage = null, object? additionalData = null, Exception? innerException = null)
        : base(errorCode, message, userFriendlyMessage, additionalData, innerException)
    {
    }

    public NotFoundException(string message)
        : base(ErrorCodes.NotFoundGeneral, message, message, null, null)
    {
    }
}
