namespace Core.Shared.Abstractions;

/// <summary>
/// Uygulama genelinde mevcut zamanı almak için standart bir sözleşme tanımlar.
/// Bu arayüz, zamanla ilgili bağımlılıkları soyutlayarak test edilebilirliği artırır.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Mevcut Eşgüdümlü Evrensel Zaman'ı (UTC) alır.
    /// </summary>
    DateTime UtcNow { get; }
}