namespace Core.Application.Common.Exceptions;

/// <summary>
/// İş kuralları ihlal edildiğinde fırlatılan hata.
/// </summary>
public class BusinessException : ApplicationException
{
    // Tek bir constructor yeterli. Gerekli tüm bilgileri base'e (ApplicationException) gönderir.
    public BusinessException(
        string message,
        string? userFriendlyMessage = null,
        string? errorCode = null,
        object? additionalData = null)
        : base(message, userFriendlyMessage, errorCode, additionalData)
    {
    }

    // Hata listesinden oluşturan pratik bir constructor
    public BusinessException(
        IEnumerable<string> errors,
        string? userFriendlyMessage = null,
        string? errorCode = null,
        object? additionalData = null)
        : base(string.Join(Environment.NewLine, errors ?? Enumerable.Empty<string>()), userFriendlyMessage, errorCode, additionalData)
    {
    }
}