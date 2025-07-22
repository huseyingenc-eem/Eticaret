namespace Core.Shared.Utilities;

/// <summary>
/// Uygulama genelinde kullanılabilen, yeniden kullanılabilir doğrulama yardımcı metotları içerir.
/// </summary>
public static class ValidationUtility
{
    /// <summary>
    /// Verilen TC Kimlik Numarasının algoritmasını doğrular.
    /// Not: Bu, sadece algoritmayı kontrol eder, bir kişinin geçerli bir vatandaşa
    /// ait olup olmadığını MERNİS üzerinden teyit etmez.
    /// </summary>
    /// <param name="tcKimlikNo">Doğrulanacak 11 haneli TC Kimlik Numarası.</param>
    /// <returns>TC Kimlik No geçerli formattaysa true, aksi takdirde false.</returns>
    public static bool IsValidTcKimlikNo(string? tcKimlikNo)
    {
        if (string.IsNullOrWhiteSpace(tcKimlikNo) || tcKimlikNo.Length != 11 || !tcKimlikNo.All(char.IsDigit))
        {
            return false;
        }

        long kimlikNo = long.Parse(tcKimlikNo);
        if (kimlikNo <= 0) return false;

        int[] digits = tcKimlikNo.Select(c => c - '0').ToArray();

        int tekToplam = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        int ciftToplam = digits[1] + digits[3] + digits[5] + digits[7];

        int onuncuHane = ((tekToplam * 7) - ciftToplam) % 10;
        if (digits[9] != onuncuHane)
        {
            return false;
        }

        int onbirinciHane = (tekToplam + ciftToplam + digits[9]) % 10;
        if (digits[10] != onbirinciHane)
        {
            return false;
        }

        return true;
    }
}