namespace Core.Shared.Exceptions;

/// <summary>
/// İş kuralları veya beklenen uygulama akışı dışındaki durumlar için fırlatılan özel exception sınıfı.
/// Genellikle kullanıcıya gösterilebilecek veya loglanabilecek ek bilgiler içerir.
/// </summary>
public class BusinessException : CoreException // CoreException'dan türetildi
{
    /// <summary>
    /// Sadece bir mesaj ile BusinessException oluşturur.
    /// </summary>
    /// <param name="message">Hata mesajı.</param>
    public BusinessException(string message)
        : base(message) { }

    /// <summary>
    /// Bir mesaj ve iç exception ile BusinessException oluşturur.
    /// </summary>
    /// <param name="message">Hata mesajı.</param>
    /// <param name="innerException">Bu exception'a neden olan iç exception.</param>
    public BusinessException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Detaylı bilgilerle BusinessException oluşturur (iç exception olmadan).
    /// Bu constructor, CoreException'ın (string, string?, string?, object?) imzalı bir constructor'a sahip olduğunu varsayar.
    /// </summary>
    /// <param name="message">Hata mesajı.</param>
    /// <param name="errorCode">Özel hata kodu.</param>
    /// <param name="userFriendlyMessage">Kullanıcıya gösterilebilecek dostane mesaj.</param>
    /// <param name="additionalData">Hata ile ilgili ek veri.</param>
    public BusinessException(
        string message,
        string? errorCode = null,
        string? userFriendlyMessage = null,
        object? additionalData = null)
        : base(message, errorCode, userFriendlyMessage, additionalData) // Bu base çağrısının CoreException'da karşılığı olduğu varsayılır.
    {
    }

    /// <summary>
    /// Detaylı bilgiler ve iç exception ile BusinessException oluşturur.
    /// CoreException'ın (string, Exception, string?, string?, object?) imzalı bir constructor'ı olmadığı
    /// veya uyumsuz olduğu için, daha basit bir base constructor çağrılır ve özellikler ayrıca atanır.
    /// Bu, CoreException'ın ErrorCode, UserFriendlyMessage, AdditionalData özelliklerine sahip olduğunu
    /// ve bu özelliklerin (en azından protected set ile) atanabilir olduğunu varsayar.
    /// </summary>
    /// <param name="message">Hata mesajı.</param>
    /// <param name="innerException">Bu exception'a neden olan iç exception.</param>
    /// <param name="errorCode">Özel hata kodu.</param>
    /// <param name="userFriendlyMessage">Kullanıcıya gösterilebilecek dostane mesaj.</param>
    /// <param name="additionalData">Hata ile ilgili ek veri.</param>
    

    /// <summary>
    /// Bir string listesinden (genellikle validasyon veya IdentityResult hataları) BusinessException oluşturur.
    /// Hata mesajları yeni satırlarla birleştirilir.
    /// Bu constructor, CoreException'ın (string, string?, string?, object?) imzalı bir constructor'a sahip olduğunu varsayar.
    /// </summary>
    /// <param name="errors">Hata mesajlarının listesi.</param>
    /// <param name="errorCode">Özel hata kodu.</param>
    /// <param name="userFriendlyMessage">Kullanıcıya gösterilebilecek dostane mesaj (opsiyonel, genellikle birleştirilmiş mesaj kullanılır).</param>
    /// <param name="additionalData">Hata ile ilgili ek veri.</param>
    public BusinessException(
        IEnumerable<string> errors,
        string? errorCode = null,
        string? userFriendlyMessage = null,
        object? additionalData = null)
        : base(string.Join(Environment.NewLine, errors ?? Enumerable.Empty<string>()), errorCode, userFriendlyMessage, additionalData)
    {
    }
}
