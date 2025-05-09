namespace Core.CrossCuttingConcerns.Logger.Models;

/// <summary>
/// Loglama için detaylı bilgileri içeren model sınıfı.
/// </summary>
public class LogDetail
{
    /// <summary>
    /// Loglanan metodun ait olduğu sınıfın tam adı (namespace dahil).
    /// </summary>
    public string? ClassFullName { get; set; }

    /// <summary>
    /// Loglanan metodun adı.
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// İsteği yapan kullanıcının adı veya "Anonymous".
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    /// İsteği yapan kullanıcının ID'si (eğer varsa).
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Loglanan metodun parametreleri.
    /// </summary>
    public List<LogParameter>? Parameters { get; set; }

    /// <summary>
    /// HTTP isteğinin yolu (eğer bir HTTP isteği bağlamında loglanıyorsa).
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// HTTP isteğinin metodu (GET, POST vb.).
    /// </summary>
    public string? RequestMethod { get; set; }

    /// <summary>
    /// İsteği yapan istemcinin IP adresi.
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// Logun oluşturulduğu zaman damgası (UTC).
    /// </summary>
    public DateTime LogTime { get; set; }

    /// <summary>
    /// İşlem başarılıysa, metodun dönüş değeri (isteğe bağlı).
    /// </summary>
    public object? Response { get; set; }

    /// <summary>
    /// Bir hata oluştuysa, hatanın tipi (örn: System.NullReferenceException).
    /// </summary>
    public string? ExceptionType { get; set; }

    /// <summary>
    /// Bir hata oluştuysa, hatanın ana mesajı.
    /// </summary>
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// Bir hata oluştuysa, hatanın yığın izi (stack trace).
    /// </summary>
    public string? ExceptionStackTrace { get; set; }

    /// <summary>
    /// Bir hata oluştuysa, hatanın tüm detaylarını içeren metin (genellikle Exception.ToString()).
    /// </summary>
    public string? ExceptionDetails { get; set; }

    /// <summary>
    /// Özellikle validasyon hataları gibi durumlarda ek hata detaylarını tutmak için.
    /// Anahtar-değer çiftleri veya serileştirilmiş bir nesne olabilir.
    /// </summary>
    public object? AdditionalExceptionData { get; set; }

    public LogDetail()
    {
        LogTime = DateTime.UtcNow;
        Parameters = new List<LogParameter>();
    }
}
