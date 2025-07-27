using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.UserRoles.Rules;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Commands.Delete;

#region Komut Sınıfı (Command Class)

/// <summary>
/// Kullanıcıdan belirli bir rolü kaldırma işlemini temsil eden komut.
/// ITransactionalRequest: Bu işlemin bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: İşlem başarılı olduğunda ilgili önbelleği temizler.
/// </summary>
public class DeleteUserRoleCommand : IRequest<DeleteUserRoleResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    #region Komut Parametreleri (Command Parameters)

    /// <summary>
    /// Rolü kaldırılacak kullanıcının ID'si.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Kaldırılacak rolün ID'si.
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    #endregion

    #region Önbellek Ayarları (Cache Settings)

    public string? CacheKey => $"user-roles:{UserId}";
    public string? CacheGroupKey => "UserRoles";
    public bool BypassCache => false;

    #endregion
}

#endregion

#region Komut İşleyici (Command Handler)

/// <summary>
/// DeleteUserRoleCommand komutunu işleyen handler sınıfı.
/// Kullanıcıdan güvenli bir şekilde rol kaldırır.
/// </summary>
public class DeleteUserRoleCommandHandler : IRequestHandler<DeleteUserRoleCommand, DeleteUserRoleResponseDto>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserRolesBusinessRules _userRolesBusinessRules;

    #endregion

    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// DeleteUserRoleCommandHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="userManager">ASP.NET Identity kullanıcı yöneticisi.</param>
    /// <param name="roleManager">ASP.NET Identity rol yöneticisi.</param>
    /// <param name="userRolesBusinessRules">UserRoles iş kuralları servisi.</param>
    public DeleteUserRoleCommandHandler(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        UserRolesBusinessRules userRolesBusinessRules)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userRolesBusinessRules = userRolesBusinessRules;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    /// <summary>
    /// Kullanıcı rol silme komutunu işler.
    /// </summary>
    /// <param name="request">Silme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Silme işlemi sonucu bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<DeleteUserRoleResponseDto> Handle(DeleteUserRoleCommand request, CancellationToken cancellationToken)
    {
        // 1. İstek parametrelerini doğrula
        ValidateRequest(request);

        // 2. Kullanıcıyı getir ve doğrula
        User user = await _userRolesBusinessRules.CheckUserExistsAsync(request.UserId, cancellationToken);

        // 3. Rolü getir ve doğrula
        IdentityRole role = await _userRolesBusinessRules.CheckRoleExistsAsync(request.RoleId, cancellationToken);

        // 4. İş kurallarını kontrol et
        await ValidateBusinessRulesAsync(user, role, cancellationToken);

        // 5. Rolü kullanıcıdan kaldır
        await RemoveRoleFromUserAsync(user, role);

        // 6. Güncel rol listesini al
        var remainingRoles = await _userManager.GetRolesAsync(user);

        // 7. Yanıt oluştur ve döndür
        return CreateResponseDto(user, role, remainingRoles);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// İstek parametrelerinin geçerliliğini kontrol eder.
    /// </summary>
    /// <param name="request">Doğrulanacak istek.</param>
    private static void ValidateRequest(DeleteUserRoleCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new BusinessException(
                message: "User ID cannot be null or empty.",
                userFriendlyMessage: "Kullanıcı ID'si boş olamaz.",
                errorCode: "INVALID_USER_ID"
            );
        }

        if (string.IsNullOrWhiteSpace(request.RoleId))
        {
            throw new BusinessException(
                message: "Role ID cannot be null or empty.",
                userFriendlyMessage: "Rol ID'si boş olamaz.",
                errorCode: "INVALID_ROLE_ID"
            );
        }
    }

    /// <summary>
    /// Silme işlemi öncesi iş kurallarını kontrol eder.
    /// </summary>
    /// <param name="user">Kullanıcı entity'si.</param>
    /// <param name="role">Kaldırılacak rol entity'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ValidateBusinessRulesAsync(User user, IdentityRole role, CancellationToken cancellationToken)
    {
        // Kullanıcının bu role sahip olduğunu kontrol et
        await _userRolesBusinessRules.CheckUserHasRoleAsync(user, role.Name!, cancellationToken);

        // Kritik rollerin kaldırılmasını engelle
        _userRolesBusinessRules.CheckRoleCanBeRemoved(role.Name!);

        // Kullanıcının en az bir role sahip kalmasını sağla
        await _userRolesBusinessRules.CheckUserWillHaveAtLeastOneRoleAsync(user, role.Name!, cancellationToken);
    }

    /// <summary>
    /// Kullanıcıdan rolü kaldırır.
    /// </summary>
    /// <param name="user">Kullanıcı entity'si.</param>
    /// <param name="role">Kaldırılacak rol entity'si.</param>
    /// <exception cref="BusinessException">Kaldırma işlemi başarısız olursa fırlatılır.</exception>
    private async Task RemoveRoleFromUserAsync(User user, IdentityRole role)
    {
        IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role.Name!);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description).ToList();
            throw new BusinessException(
                message: $"Failed to remove role '{role.Name}' from user '{user.UserName}'. Errors: {string.Join(", ", errors)}",
                userFriendlyMessage: $"Rol kaldırma işlemi başarısız oldu: {string.Join(Environment.NewLine, errors)}",
                errorCode: "ROLE_REMOVAL_FAILED"
            );
        }
    }

    /// <summary>
    /// Silme işlemi sonucu için yanıt DTO'sunu oluşturur.
    /// </summary>
    /// <param name="user">Kullanıcı entity'si.</param>
    /// <param name="removedRole">Kaldırılan rol entity'si.</param>
    /// <param name="remainingRoles">Kalan rol listesi.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private static DeleteUserRoleResponseDto CreateResponseDto(User user, IdentityRole removedRole, IList<string> remainingRoles)
    {
        return new DeleteUserRoleResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName!,
            RemovedRoleId = removedRole.Id,
            RemovedRoleName = removedRole.Name!,
            RemainingRoles = remainingRoles.ToList(),
            Message = $"'{removedRole.Name}' rolü kullanıcıdan başarıyla kaldırıldı.",
            IsSuccess = true
        };
    }

    #endregion
}

#endregion