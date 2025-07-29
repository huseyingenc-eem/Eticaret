using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;

/// <summary>
/// İstenen bir kaynak bulunamadığında fırlatılan hata.
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(
        string message,
        string? userFriendlyMessage = null,
        string? errorCode = null,
        object? additionalData = null)
        : base(message, errorCode ?? ApplicationErrorCodes.NotFoundGeneral, userFriendlyMessage, additionalData)
    {
    }
}