using Core.Application.Common.Exceptions;
using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Application.Features.UserRoles.Rules;

/// <summary>
/// UserRoles (Kullanıcı-Rol) işlemleri için ortak iş kurallarını içeren sınıf.
/// Bu sınıf, tüm kullanıcı-rol operasyonlarında kullanılan doğrulama ve kontrol işlemlerini merkezi olarak yönetir.
/// </summary>
public class UserRolesBusinessRules
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    #endregion

    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// UserRolesBusinessRules sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="userManager">ASP.NET Identity kullanıcı yöneticisi.</param>
    /// <param name="roleManager">ASP.NET Identity rol yöneticisi.</param>
    public UserRolesBusinessRules(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    #endregion

    #region Kullanıcı Doğrulama Kuralları (User Validation Rules)

    /// <summary>
    /// Kullanıcının varlığını kontrol eder.
    /// </summary>
    /// <param name="userId">Kontrol edilecek kullanıcının ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Doğrulanmış kullanıcı entity'si.</returns>
    /// <exception cref="NotFoundException">Kullanıcı bulunamadığında fırlatılır.</exception>
    public async Task<User> CheckUserExistsAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new BusinessException(
                message: "User ID cannot be null or empty.",
                userFriendlyMessage: "Kullanıcı ID'si boş olamaz.",
                errorCode: "INVALID_USER_ID"
            );
        }

        User? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(
                message: $"User with ID '{userId}' was not found.",
                userFriendlyMessage: $"Kullanıcı bulunamadı (ID: {userId}).",
                errorCode: "USER_NOT_FOUND"
            );
        }

        return user;
    }

    /// <summary>
    /// Birden fazla kullanıcının varlığını toplu olarak kontrol eder.
    /// </summary>
    /// <param name="userIds">Kontrol edilecek kullanıcı ID'lerinin listesi.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Bulunamayan kullanıcı ID'lerinin listesi.</returns>
    public async Task<List<string>> CheckMultipleUsersExistAsync(List<string> userIds, CancellationToken cancellationToken = default)
    {
        if (userIds == null || !userIds.Any())
        {
            return new List<string>();
        }

        var existingUsers = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        return userIds.Except(existingUsers).ToList();
    }

    #endregion

    #region Rol Doğrulama Kuralları (Role Validation Rules)

    /// <summary>
    /// Rolün varlığını kontrol eder.
    /// </summary>
    /// <param name="roleId">Kontrol edilecek rolün ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Doğrulanmış rol entity'si.</returns>
    /// <exception cref="NotFoundException">Rol bulunamadığında fırlatılır.</exception>
    public async Task<IdentityRole> CheckRoleExistsAsync(string roleId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleId))
        {
            throw new BusinessException(
                message: "Role ID cannot be null or empty.",
                userFriendlyMessage: "Rol ID'si boş olamaz.",
                errorCode: "INVALID_ROLE_ID"
            );
        }

        IdentityRole? role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            throw new NotFoundException(
                message: $"Role with ID '{roleId}' was not found.",
                userFriendlyMessage: $"Rol bulunamadı (ID: {roleId}).",
                errorCode: "ROLE_NOT_FOUND"
            );
        }

        if (string.IsNullOrEmpty(role.Name))
        {
            throw new BusinessException(
                message: $"Role with ID '{roleId}' has an invalid name.",
                userFriendlyMessage: $"Bulunan rolün geçerli bir adı yok (ID: {roleId}).",
                errorCode: "INVALID_ROLE_NAME"
            );
        }

        return role;
    }

    /// <summary>
    /// Rol adından rolü kontrol eder.
    /// </summary>
    /// <param name="roleName">Kontrol edilecek rol adı.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Doğrulanmış rol entity'si.</returns>
    /// <exception cref="NotFoundException">Rol bulunamadığında fırlatılır.</exception>
    public async Task<IdentityRole> CheckRoleExistsByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new BusinessException(
                message: "Role name cannot be null or empty.",
                userFriendlyMessage: "Rol adı boş olamaz.",
                errorCode: "INVALID_ROLE_NAME"
            );
        }

        IdentityRole? role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            throw new NotFoundException(
                message: $"Role with name '{roleName}' was not found.",
                userFriendlyMessage: $"'{roleName}' adında bir rol bulunamadı.",
                errorCode: "ROLE_NOT_FOUND"
            );
        }

        return role;
    }

    #endregion

    #region Kullanıcı-Rol İlişki Kuralları (User-Role Relationship Rules)

    /// <summary>
    /// Kullanıcının belirtilen role sahip olmadığını doğrular.
    /// </summary>
    /// <param name="user">Kontrol edilecek kullanıcı.</param>
    /// <param name="roleName">Kontrol edilecek rol adı.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Kullanıcı zaten bu role sahipse fırlatılır.</exception>
    public async Task CheckUserDoesNotHaveRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        bool roleExists = userRoles.Any(x => string.Equals(x, roleName, StringComparison.OrdinalIgnoreCase));

        if (roleExists)
        {
            throw new BusinessException(
                message: $"User '{user.UserName}' already has the role '{roleName}'.",
                userFriendlyMessage: $"Kullanıcının '{roleName}' rolü zaten mevcut.",
                errorCode: "USER_ALREADY_HAS_ROLE"
            );
        }
    }

    /// <summary>
    /// Kullanıcının belirtilen role sahip olduğunu doğrular.
    /// </summary>
    /// <param name="user">Kontrol edilecek kullanıcı.</param>
    /// <param name="roleName">Kontrol edilecek rol adı.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Kullanıcı bu role sahip değilse fırlatılır.</exception>
    public async Task CheckUserHasRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        bool roleExists = userRoles.Any(x => string.Equals(x, roleName, StringComparison.OrdinalIgnoreCase));

        if (!roleExists)
        {
            throw new BusinessException(
                message: $"User '{user.UserName}' does not have the role '{roleName}'.",
                userFriendlyMessage: $"Kullanıcının '{roleName}' rolü bulunmuyor.",
                errorCode: "USER_DOES_NOT_HAVE_ROLE"
            );
        }
    }

    /// <summary>
    /// Sistemde kritik rollerin silinmesini engeller.
    /// </summary>
    /// <param name="roleName">Silinecek rol adı.</param>
    /// <exception cref="BusinessException">Kritik rol silinmeye çalışıldığında fırlatılır.</exception>
    public void CheckRoleCanBeRemoved(string roleName)
    {
        var criticalRoles = new[] { "Admin", "SuperAdmin", "SystemAdmin" };

        if (criticalRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessException(
                message: $"Critical role '{roleName}' cannot be removed from users.",
                userFriendlyMessage: $"'{roleName}' rolü kritik bir roldür ve kullanıcılardan kaldırılamaz.",
                errorCode: "CRITICAL_ROLE_CANNOT_BE_REMOVED"
            );
        }
    }

    /// <summary>
    /// Kullanıcının en az bir role sahip kalmasını sağlar.
    /// </summary>
    /// <param name="user">Kontrol edilecek kullanıcı.</param>
    /// <param name="roleToRemove">Kaldırılacak rol adı.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Kullanıcının son rolü kaldırılmaya çalışıldığında fırlatılır.</exception>
    public async Task CheckUserWillHaveAtLeastOneRoleAsync(User user, string roleToRemove, CancellationToken cancellationToken = default)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        if (userRoles.Count <= 1 && userRoles.Contains(roleToRemove, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessException(
                message: $"User '{user.UserName}' must have at least one role. Cannot remove the last role '{roleToRemove}'.",
                userFriendlyMessage: "Kullanıcının en az bir rolü olması gerekir. Son rol kaldırılamaz.",
                errorCode: "USER_MUST_HAVE_AT_LEAST_ONE_ROLE"
            );
        }
    }

    #endregion

    #region Genel Doğrulama Kuralları (General Validation Rules)

    /// <summary>
    /// Sayfalama parametrelerinin geçerliliğini kontrol eder.
    /// </summary>
    /// <param name="pageIndex">Sayfa indeksi.</param>
    /// <param name="pageSize">Sayfa boyutu.</param>
    /// <exception cref="BusinessException">Geçersiz sayfalama parametreleri için fırlatılır.</exception>
    public static void ValidatePaginationParameters(int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
        {
            throw new BusinessException(
                message: "Page index cannot be negative.",
                userFriendlyMessage: "Sayfa indeksi negatif olamaz.",
                errorCode: "INVALID_PAGE_INDEX"
            );
        }

        if (pageSize <= 0 || pageSize > 100)
        {
            throw new BusinessException(
                message: "Page size must be between 1 and 100.",
                userFriendlyMessage: "Sayfa boyutu 1 ile 100 arasında olmalıdır.",
                errorCode: "INVALID_PAGE_SIZE"
            );
        }
    }

    #endregion
}