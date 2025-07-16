namespace Core.Application.Contracts.Requests;

/// <summary>
/// Tüm CQRS Komut (Command) ve Sorgu (Query) nesneleri için temel sınıf.
/// Sisteme yapılan her istekte ortak olarak bulunması beklenen alanları içerir.
/// </summary>
public abstract class BaseRequest
{
    /// <summary>
    /// İsteğin yaşam döngüsü boyunca takip edilebilmesi için kullanılan benzersiz kimlik.
    /// Bu ID, loglarda ve dağıtık sistemlerde isteğin geçtiği tüm adımları birleştirmek için kullanılır.
    /// Eğer dış dünyadan bir CorrelationId gelmezse, constructor'da yeni bir tane oluşturulur.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// İsteği yapan kimliği doğrulanmış kullanıcının benzersiz kimliği.
    /// Bazı istekler anonim olabileceğinden bu alan nullable'dır (örn: login, register).
    /// </summary>
    public Guid? AuthenticatedUserId { get; set; }

    /// <summary>
    /// İsteğin yapıldığı coğrafi/kültürel bilgi (örn: "tr-TR", "en-US").
    /// Bu bilgi, cevapların veya işlemlerin yerelleştirilmesi için kullanılabilir.
    /// </summary>
    public string? Culture { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    protected BaseRequest()
    {
        CorrelationId = Guid.NewGuid();
    }
}