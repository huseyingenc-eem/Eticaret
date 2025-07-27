namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Tek bir iş kuralını temsil eden temel arayüz.
/// Her kural kendi sorumluluğuna sahiptir (Single Responsibility Principle).
/// </summary>
/// <typeparam name="TCommand">Kuralın uygulanacağı komut türü</typeparam>
public interface IRule<in TCommand> where TCommand : class
{
    /// <summary>
    /// Kuralın öncelik sırası. Düşük değer yüksek öncelik anlamına gelir.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Kuralın adı/tanımlayıcısı. Loglama ve hata ayıklama için kullanılır.
    /// </summary>
    string RuleName { get; }

    /// <summary>
    /// Bu kuralı çalıştırır.
    /// </summary>
    /// <param name="command">Kontrol edilecek komut</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
}