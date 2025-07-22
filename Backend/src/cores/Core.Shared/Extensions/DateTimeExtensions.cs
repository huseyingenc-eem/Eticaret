// Konum: cores/Core.Shared/Extensions/DateTimeExtensions.cs
namespace Core.Shared.Extensions;

/// <summary>
/// DateTime nesneleri için kullanışlı genişletme metotları sağlar.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Bir DateTime nesnesini, Unix zaman damgasına (1 Ocak 1970'ten bu yana geçen saniye sayısı) dönüştürür.
    /// </summary>
    /// <param name="dateTime">Dönüştürülecek tarih.</param>
    /// <returns>Unix zaman damgası olarak uzun bir tamsayı.</returns>
    public static long ToUnixEpochDate(this DateTime dateTime)
        => (long)Math.Round((dateTime.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
}