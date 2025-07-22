namespace Core.Application.Abstractions.Services;

/// <summary>
/// Nesneleri serileştirme (bir veri formatına dönüştürme) ve deserileştirme
/// (veri formatından nesneye dönüştürme) işlemleri için genel bir sözleşme tanımlar.
/// Bu arayüz, uygulamanın belirli bir serileştirme kütüphanesine (örn: System.Text.Json) doğrudan bağımlı olmasını engeller.
/// </summary>
public interface ISerializerService
{
    /// <summary>
    /// Bir byte dizisini (genellikle UTF-8 formatındaki JSON) belirtilen T tipindeki bir nesneye dönüştürür.
    /// </summary>
    /// <typeparam name="T">Dönüştürülecek nesnenin hedef tipi.</typeparam>
    /// <param name="data">Dönüştürülecek veriyi içeren byte dizisi.</param>
    /// <returns>Dönüştürülmüş T tipindeki nesne veya veri geçersizse null.</returns>
    T? Deserialize<T>(byte[] data);

    /// <summary>
    /// Belirtilen T tipindeki bir nesneyi UTF-8 formatında bir byte dizisine serileştirir.
    /// </summary>
    /// <typeparam name="T">Serileştirilecek nesnenin tipi.</typeparam>
    /// <param name="value">Serileştirilecek nesne.</param>
    /// <returns>Nesnenin serileştirilmiş halini içeren byte dizisi.</returns>
    byte[] SerializeToUtf8Bytes<T>(T value);
}