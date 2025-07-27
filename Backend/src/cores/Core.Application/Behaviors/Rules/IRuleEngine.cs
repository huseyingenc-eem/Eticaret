namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Kural motorunun temel arayüzü.
/// Her komut için ilgili kuralları çalıştırır.
/// </summary>
public interface IRuleEngine
{
    /// <summary>
    /// Belirtilen komut türü için tüm kuralları çalıştırır.
    /// </summary>
    /// <typeparam name="TCommand">Komut türü - reference type olmalı</typeparam>
    /// <param name="command">Çalıştırılacak komut</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    Task ValidateAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class;
}