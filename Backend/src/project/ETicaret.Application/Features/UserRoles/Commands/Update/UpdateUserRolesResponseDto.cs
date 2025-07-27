namespace ETicaret.Application.Features.UserRoles.Commands.Update;

/// <summary>
/// Kullanıcı rol güncelleme komutunun yanıt DTO'su.
/// İşlem sonucunda hangi rollerin eklendiği, kaldırıldığı ve güncel durumun ne olduğunu içerir.
/// </summary>
public class UpdateUserRolesResponseDto
{
    #region Kullanıcı Bilgileri (User Information)

    /// <summary>
    /// Güncellenen kullanıcının ID'si.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Güncellenen kullanıcının adı.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    #endregion

    #region İşlem Sonuçları (Operation Results)

    /// <summary>
    /// Kullanıcıya eklenen rollerin detayları ve sonuçları.
    /// </summary>
    public List<RoleOperationResult> AddedRoles { get; set; } = new();

    /// <summary>
    /// Kullanıcıdan kaldırılan rollerin detayları ve sonuçları.
    /// </summary>
    public List<RoleOperationResult> RemovedRoles { get; set; } = new();

    /// <summary>
    /// Güncelleme işlemi sonrası kullanıcının güncel rol listesi.
    /// </summary>
    public List<string> CurrentRoles { get; set; } = new();

    #endregion

    #region Genel Bilgiler (General Information)

    /// <summary>
    /// İşlem sonucu mesajı.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// İşlemin başarılı olup olmadığını belirtir.
    /// </summary>
    public bool IsSuccess { get; set; }

    #endregion
}

/// <summary>
/// Tek bir rol işleminin (ekleme veya kaldırma) sonucunu temsil eder.
/// </summary>
public class RoleOperationResult
{
    /// <summary>
    /// İşlem yapılan rolün ID'si.
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// İşlem yapılan rolün adı.
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// İşlemin başarılı olup olmadığını belirtir.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// İşlem sırasında oluşan hata mesajları (varsa).
    /// </summary>
    public List<string> Errors { get; set; } = new();
}