using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string errorCode, string message, string? userFriendlyMessage = null, object? additionalData = null, Exception? innerException = null)
        : base(errorCode, message, userFriendlyMessage, additionalData, innerException)
    {
    }

    public NotFoundException(string message)
        : base(ApplicationErrorCodes.NotFoundGeneral, message, message, null, null)
    {
    }
}
