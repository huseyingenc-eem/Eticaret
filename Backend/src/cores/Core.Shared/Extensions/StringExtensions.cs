// Konum: cores/Core.Shared/Extensions/StringExtensions.cs
namespace Core.Shared.Extensions;

/// <summary>
/// String (metin) nesneleri için genel amaçlı genişletme metotları içerir.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Bir string'in null, boş veya sadece boşluk karakterlerinden oluşup oluşmadığını kontrol eder.
    /// Bu, 'string.IsNullOrWhiteSpace(value)' metodunun bir genişletme metodu versiyonudur.
    /// </summary>
    /// <param name="value">Kontrol edilecek metin.</param>
    /// <returns>Metin null veya boşluk ise true, aksi takdirde false.</returns>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
}