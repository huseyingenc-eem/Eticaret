namespace Core.Domain.Exceptions;

/// <summary>
/// Domain katmanında bir iş kuralı ihlal edildiğinde veya geçersiz bir durum oluştuğunda fırlatılan,
/// yapısal veri taşıyabilen özel bir istisna.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Uygulama genelinde kullanılacak, makine tarafından okunabilir benzersiz hata kodunu alır.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Kullanıcı dostu mesaj
    /// </summary>
    public string? UserFriendlyMessage { get; }

    /// <summary>
    /// Hata hakkında ek bağlamsal detaylar taşıyan nesneyi alır.
    /// </summary>
    public object? Details { get; }

    /// <summary>
    /// DomainException sınıfının yeni bir örneğini belirtilen mesaj, hata kodu ve detaylarla başlatır.
    /// </summary>
    /// <param name="message">İstisnanın nedenini açıklayan geliştirici dostu hata mesajı.</param>
    /// <param name="errorCode">Makine tarafından okunabilir benzersiz hata kodu.</param>
    /// <param name="userFriendlyMessage">Kullanıcı dostu mesaj (opsiyonel)</param>
    /// <param name="details">Hata ile ilgili ek bağlamsal veri.</param>
    public DomainException(string message, string errorCode, string? userFriendlyMessage = null, object? details = null)
        : base(message)
    {
        ErrorCode = errorCode;
        UserFriendlyMessage = userFriendlyMessage;
        Details = details;
    }

    /// <summary>
    /// DomainException sınıfının yeni bir örneğini belirtilen mesaj, hata kodu, iç istisna ve detaylarla başlatır.
    /// </summary>
    /// <param name="message">İstisnanın nedenini açıklayan geliştirici dostu hata mesajı.</param>
    /// <param name="errorCode">Makine tarafından okunabilir benzersiz hata kodu.</param>
    /// <param name="innerException">Mevcut istisnanın nedeni olan istisna.</param>
    /// <param name="userFriendlyMessage">Kullanıcı dostu mesaj (opsiyonel)</param>
    /// <param name="details">Hata ile ilgili ek bağlamsal veri.</param>
    public DomainException(string message, string errorCode, Exception innerException, string? userFriendlyMessage = null, object? details = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        UserFriendlyMessage = userFriendlyMessage;
        Details = details;
    }
}