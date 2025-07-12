namespace Core.Shared.Exceptions;

/// <summary>
/// Uygulama genelinde kullanılacak temel özel exception sınıfı.
/// Hata kodu ve isteğe bağlı olarak ek detaylar içerebilir.
/// </summary>
// Core.CrossCuttingConcerns.Exceptions.CoreException.cs (Tahmini Yapı)
public class CoreException : Exception
{
    public string? ErrorCode { get; }
    public string? UserFriendlyMessage { get; }
    public object? AdditionalData { get; set; } // Veya protected set

    public CoreException(string message)
        : base(message) { }

    public CoreException(string message, Exception? innerException)
        : base(message, innerException) { }

    // Bu constructor'lar CoreException'da olmayabilir, bu durumda BusinessException'da
    // bu property'leri base'e mesaj ve innerException'ı yolladıktan sonra set etmemiz gerekir.
    public CoreException(
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