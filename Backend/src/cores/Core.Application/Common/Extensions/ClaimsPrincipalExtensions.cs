using Core.Application.Common.Exceptions;
using System.Security.Claims;

namespace Core.Application.Common.Extensions;


/// <summary>
/// ClaimsPrincipal sınıfını genişleterek kullanıcı token'ı (JWT) içindeki
/// bilgilere daha kolay erişim sağlayan yardımcı metotlar içerir.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Kullanıcının token'ında bulunan 'NameIdentifier' claim'ini bulur ve Guid olarak döndürür.
    /// Bu metot, kullanıcı kimliğinin token içinde mutlaka bulunmasını ve Guid formatında olmasını zorunlu kılar.
    /// 
    /// 
    /// <exception cref="AuthorizationException">Eğer 'NameIdentifier' claim'i token'da bulunamazsa veya geçersiz bir formattaysa  fırlatılır.
    /// </exception>
    /// <example>
    /// Bir Controller içinde kullanımı:
    /// <code>
    /// [HttpGet("my-profile")]
    /// public IActionResult GetMyProfile()
    /// {
    ///     // 'User' özelliği, o anki isteği yapan kullanıcının ClaimsPrincipal'ını temsil eder.
    ///     Guid userId = User.GetUserId(); 
    ///     var userProfile = _userService.GetProfile(userId);
    ///     return Ok(userProfile);
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="claimsPrincipal">Genişletilecek ClaimsPrincipal nesnesi (genellikle Controller'daki User özelliği).</param>
    /// <returns>Kullanıcının Guid formatındaki ID'si.</returns>
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? throw new AuthorizationException("User ID claim ('NameIdentifier') not found in token.");

        if (Guid.TryParse(userIdClaim, out Guid userId))
        {
            return userId;
        }

        throw new AuthorizationException("Invalid User ID format in token. Expected a valid GUID.");
    }
    public static string? FindFirstValue(this ClaimsPrincipal principal, string claimType)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        var first = principal.FindFirst(claimType);
        return first?.Value;
    }


    /// <summary>
    /// Kullanıcının token'ında bulunan tüm 'Role' claim'lerini bir string listesi olarak döndürür.
    /// 
    /// <example>
    /// <code>
    /// List&lt;string&gt; userRoles = User.GetUserRoles();
    /// if (userRoles.Contains("Admin"))
    /// {
    ///     // Kullanıcı bir admin.
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="claimsPrincipal">Genişletilecek ClaimsPrincipal nesnesi.</param>
    /// <returns>Kullanıcının sahip olduğu rollerin bir listesi. Eğer hiç rolü yoksa boş bir liste döner.</returns>
    public static List<string> GetUserRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindAll(ClaimTypes.Role)
                              .Select(r => r.Value)
                              .ToList();
    }

    // IsUserInRole metodu, .NET'in kendi IsInRole metodu ile aynı işi yaptığı için gereksizdir ve silinmiştir.
    // Direkt olarak `User.IsInRole("Admin")` şeklinde kullanılmalıdır.
}