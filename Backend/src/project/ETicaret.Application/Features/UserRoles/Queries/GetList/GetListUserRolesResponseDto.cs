namespace ETicaret.Application.Features.UserRoles.Queries.GetList;

/// <summary>
/// Kullanıcı-rol listesi sorgusu için yanıt DTO'su.
/// Her kullanıcının temel bilgileri ve sahip olduğu rollerin detaylı listesini içerir.
/// </summary>
public class GetListUserRolesResponseDto
{
    #region Kullanıcı Bilgileri (User Information)

    /// <summary>
    /// Kullanıcının benzersiz kimliği.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcı adı.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email adresi.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının adı.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının soyadı.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Tam adı (Ad + Soyad).
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının bulunduğu şehir.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Email adresinin onaylanıp onaylanmadığı.
    /// </summary>
    public bool EmailConfirmed { get; set; }

    #endregion

    #region Rol Bilgileri (Role Information)

    /// <summary>
    /// Kullanıcının sahip olduğu rollerin detaylı listesi.
    /// </summary>
    public List<UserRoleDetailDto> Roles { get; set; } = new();

    /// <summary>
    /// Kullanıcının sahip olduğu rol sayısı.
    /// </summary>
    public int RoleCount { get; set; }

    /// <summary>
    /// Rollerin virgülle ayrılmış string hali (UI'da kolay gösterim için).
    /// </summary>
    public string RoleNames { get; set; } = string.Empty;

    #endregion
}

/// <summary>
/// Kullanıcının sahip olduğu bir rolün detay bilgilerini içeren DTO.
/// </summary>
public class UserRoleDetailDto
{
    /// <summary>
    /// Rolün benzersiz kimliği.
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// Rol adı.
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
}