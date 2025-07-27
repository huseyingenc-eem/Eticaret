namespace ETicaret.Application.Features.UserRoles.Commands.Delete;

/// <summary>
/// Kullanıcı rol silme komutunun yanıt DTO'su.
/// Silme işlemi sonucunda hangi rolün kaldırıldığını ve güncel durumu içerir.
/// </summary>
public class DeleteUserRoleResponseDto
{
    #region Kullanıcı Bilgileri (User Information)

    /// <summary>
    /// Rolü kaldırılan kullanıcının ID'si.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Rolü kaldırılan kullanıcının adı.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    #endregion

    #region Kaldırılan Rol Bilgileri (Removed Role Information)

    /// <summary>
    /// Kaldırılan rolün ID'si.
    /// </summary>
    public string RemovedRoleId { get; set; } = string.Empty;

    /// <summary>
    /// Kaldırılan rolün adı.
    /// </summary>
    public string RemovedRoleName { get; set; } = string.Empty;

    #endregion

    #region Güncel Durum (Current Status)

    /// <summary>
    /// Silme işlemi sonrası kullanıcının kalan rol listesi.
    /// </summary>
    public List<string> RemainingRoles { get; set; } = new();

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