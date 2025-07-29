using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;

/// <summary>
/// İş kuralları ihlal edildiğinde fırlatılan hata.
/// </summary>
public class BusinessException : ApplicationException
{
    public BusinessException(
        string message,
        string? userFriendlyMessage = null,
        string? errorCode = null,
        object? additionalData = null)
        : base(message, errorCode ?? ApplicationErrorCodes.BusinessRuleViolation, userFriendlyMessage, additionalData)
    {
    }

    // Hata listesinden oluşturan pratik bir constructor
    public BusinessException(
        IEnumerable<string> errors,
        string? userFriendlyMessage = null,
        string? errorCode = null,
        object? additionalData = null)
        : base(string.Join(Environment.NewLine, errors ?? Enumerable.Empty<string>()),
               errorCode ?? ApplicationErrorCodes.BusinessRuleViolation,
               userFriendlyMessage,
               additionalData)
    {
    }
}