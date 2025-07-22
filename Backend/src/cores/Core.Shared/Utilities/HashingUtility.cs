using System.Security.Cryptography;
using System.Text;

namespace Core.Shared.Utilities;

/// <summary>
/// Parola hashleme ve doğrulama işlemleri için yardımcı metotlar sağlar.
/// </summary>
public static class HashingUtility
{
    /// <summary>
    /// Verilen bir parolayı HMACSHA512 algoritması kullanarak hash'ler ve salt değerini oluşturur.
    /// </summary>
    /// <param name="password">Hash'lenecek parola.</param>
    /// <param name="passwordHash">Oluşturulan hash değeri (çıktı).</param>
    /// <param name="passwordSalt">Oluşturulan salt değeri (çıktı).</param>
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    /// <summary>
    /// Verilen bir parolanın, daha önce saklanmış olan hash ve salt ile eşleşip eşleşmediğini doğrular.
    /// </summary>
    /// <param name="password">Doğrulanacak parola.</param>
    /// <param name="passwordHash">Saklanmış parola hash'i.</param>
    /// <param name="passwordSalt">Saklanmış parola salt'ı.</param>
    /// <returns>Parola eşleşiyorsa true, aksi takdirde false.</returns>
    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
}