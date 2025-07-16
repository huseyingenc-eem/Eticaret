namespace Core.Application.Common.Exceptions;

/// <summary>
/// Uygulama (Application) katmanında meydana gelen ve ele alınması gereken hatalar için temel sınıf.
/// Yapısal hata bilgileri taşır.
/// </summary>
public class ApplicationException : Exception
{
    public string? ErrorCode { get; }
    public string? UserFriendlyMessage { get; }
    public object? AdditionalData { get; set; } // Veya protected set

    public ApplicationException(string message)
        : base(message) { }

    public ApplicationException(string message, Exception? innerException)
        : base(message, innerException) { }

    // Bu constructor'lar CoreException'da olmayabilir, bu durumda BusinessException'da
    // bu property'leri base'e mesaj ve innerException'ı yolladıktan sonra set etmemiz gerekir.
    public ApplicationException(
        string message,
        string? errorCode = null,
        string? userFriendlyMessage = null,
        object? additionalData = null,
        Exception? innerException = null) // InnerException sona eklendi
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        UserFriendlyMessage = userFriendlyMessage;
        AdditionalData = additionalData;
    }
}