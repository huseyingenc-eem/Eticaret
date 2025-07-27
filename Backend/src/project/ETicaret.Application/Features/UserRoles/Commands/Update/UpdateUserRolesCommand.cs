using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.UserRoles.Rules;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Commands.Update;

#region Komut Sınıfı (Command Class)

/// <summary>
/// Kullanıcının rollerini güncelleme (ekleme/çıkarma) işlemini temsil eden komut.
/// ITransactionalRequest: Bu işlemin bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: İşlem başarılı olduğunda ilgili önbelleği temizler.
/// </summary>
public class UpdateUserRolesCommand : IRequest<UpdateUserRolesResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    #region Komut Parametreleri (Command Parameters)

    /// <summary>
    /// Rolleri güncellenecek kullanıcının ID'si.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcıya eklenecek rol ID'lerinin listesi.
    /// </summary>
    public List<string> RolesToAdd { get; set; } = new();

    /// <summary>
    /// Kullanıcıdan çıkarılacak rol ID'lerinin listesi.
    /// </summary>
    public List<string> RolesToRemove { get; set; } = new();

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
/// UpdateUserRolesCommand komutunu işleyen handler sınıfı.
/// Kullanıcının rol listesini güvenli bir şekilde günceller.
/// </summary>
public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, UpdateUserRolesResponseDto>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserRolesBusinessRules _userRolesBusinessRules;

    #endregion

    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// UpdateUserRolesCommandHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="userManager">ASP.NET Identity kullanıcı yöneticisi.</param>
    /// <param name="roleManager">ASP.NET Identity rol yöneticisi.</param>
    /// <param name="userRolesBusinessRules">UserRoles iş kuralları servisi.</param>
    public UpdateUserRolesCommandHandler(
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
    /// Kullanıcı rol güncelleme komutunu işler.
    /// </summary>
    /// <param name="request">Güncelleme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Güncelleme sonucu bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<UpdateUserRolesResponseDto> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        // 1. İstek parametrelerini doğrula
        ValidateRequest(request);

        // 2. Kullanıcıyı getir ve doğrula
        User user = await _userRolesBusinessRules.CheckUserExistsAsync(request.UserId, cancellationToken);

        // 3. Rolleri doğrula
        var rolesToAdd = await ValidateAndGetRolesToAddAsync(request.RolesToAdd, cancellationToken);
        var rolesToRemove = await ValidateAndGetRolesToRemoveAsync(request.RolesToRemove, cancellationToken);

        // 4. İş kurallarını kontrol et
        await ValidateBusinessRulesAsync(user, rolesToAdd, rolesToRemove, cancellationToken);

        // 5. Rolleri kaldır
        var removeResults = await RemoveRolesFromUserAsync(user, rolesToRemove);

        // 6. Rolleri ekle
        var addResults = await AddRolesToUserAsync(user, rolesToAdd);

        // 7. Güncel rol listesini al
        var currentRoles = await _userManager.GetRolesAsync(user);

        // 8. Yanıt oluştur ve döndür
        return CreateResponseDto(user, addResults, removeResults, currentRoles);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// İstek parametrelerinin geçerliliğini kontrol eder.
    /// </summary>
    /// <param name="request">Doğrulanacak istek.</param>
    private static void ValidateRequest(UpdateUserRolesCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new BusinessException(
                message: "User ID cannot be null or empty.",
                userFriendlyMessage: "Kullanıcı ID'si boş olamaz.",
                errorCode: "INVALID_USER_ID"
            );
        }

        if ((request.RolesToAdd == null || !request.RolesToAdd.Any()) &&
            (request.RolesToRemove == null || !request.RolesToRemove.Any()))
        {
            throw new BusinessException(
                message: "At least one role operation (add or remove) must be specified.",
                userFriendlyMessage: "En az bir rol işlemi (ekleme veya kaldırma) belirtilmelidir.",
                errorCode: "NO_ROLE_OPERATION_SPECIFIED"
            );
        }
    }

    /// <summary>
    /// Eklenecek rollerin geçerliliğini kontrol eder ve döndürür.
    /// </summary>
    /// <param name="roleIds">Eklenecek rol ID'leri.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Doğrulanmış rol listesi.</returns>
    private async Task<List<IdentityRole>> ValidateAndGetRolesToAddAsync(List<string> roleIds, CancellationToken cancellationToken)
    {
        var roles = new List<IdentityRole>();

        if (roleIds == null || !roleIds.Any())
            return roles;

        foreach (var roleId in roleIds.Distinct())
        {
            var role = await _userRolesBusinessRules.CheckRoleExistsAsync(roleId, cancellationToken);
            roles.Add(role);
        }

        return roles;
    }

    /// <summary>
    /// Kaldırılacak rollerin geçerliliğini kontrol eder ve döndürür.
    /// </summary>
    /// <param name="roleIds">Kaldırılacak rol ID'leri.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Doğrulanmış rol listesi.</returns>
    private async Task<List<IdentityRole>> ValidateAndGetRolesToRemoveAsync(List<string> roleIds, CancellationToken cancellationToken)
    {
        var roles = new List<IdentityRole>();

        if (roleIds == null || !roleIds.Any())
            return roles;

        foreach (var roleId in roleIds.Distinct())
        {
            var role = await _userRolesBusinessRules.CheckRoleExistsAsync(roleId, cancellationToken);
            roles.Add(role);
        }

        return roles;
    }

    /// <summary>
    /// Güncelleme işlemi öncesi iş kurallarını kontrol eder.
    /// </summary>
    /// <param name="user">Kullanıcı entity'si.</param>
    /// <param name="rolesToAdd">Eklenecek roller.</param>
    /// <param name="rolesToRemove">Kaldırılacak roller.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ValidateBusinessRulesAsync(
        User user,
        List<IdentityRole> rolesToAdd,
        List<IdentityRole> rolesToRemove,
        CancellationToken cancellationToken)
    {
        // Eklenecek rollerin kullanıcıda zaten olmadığını kontrol et
        foreach (var role in rolesToAdd)
        {
            await _userRolesBusinessRules.CheckUserDoesNotHaveRoleAsync(user, role.Name!, cancellationToken);
        }

        // Kaldırılacak rollerin kullanıcıda olduğunu kontrol et
        foreach (var role in rolesToRemove)
        {
            await _userRolesBusinessRules.CheckUserHasRoleAsync(user, role.Name!, cancellationToken);
            _userRolesBusinessRules.CheckRoleCanBeRemoved(role.Name!);
        }

        // Kullanıcının en az bir role sahip kalacağını kontrol et
        if (rolesToRemove.Any())
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemoveNames = rolesToRemove.Select(r => r.Name!).ToList();
            var rolesToAddNames = rolesToAdd.Select(r => r.Name!).ToList();

            var finalRoleCount = currentRoles.Count - rolesToRemoveNames.Count + rolesToAddNames.Count;

            if (finalRoleCount <= 0)
            {
                throw new BusinessException(
                    message: "User must have at least one role after the update operation.",
                    userFriendlyMessage: "Güncelleme işlemi sonrası kullanıcının en az bir rolü olması gerekir.",
                    errorCode: "USER_MUST_HAVE_AT_LEAST_ONE_ROLE"
                );
            }
        }
    }

    /// <summary>
    /// Kullanıcıdan rolleri kaldırır.
    /// </summary>
    /// <param name="user">Kullanıcı entity'si.</param>
    /// <param name="rolesToRemove">Kaldırılacak roller.</param>
    /// <returns>Kaldırma işlemi sonuçları.</returns>
    private async Task<List<RoleOperationResult>> RemoveRolesFromUserAsync(User user, List<IdentityRole> rolesToRemove)
    {
        var results = new List<RoleOperationResult>();

        foreach (var role in rolesToRemove)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
            results.Add(new RoleOperationResult
            {
                RoleId = role.Id,
                RoleName = role.Name!,
                IsSuccess = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        return results;
    }

    /// <summary>
    /// Kullanıcıya rolleri ekler.
    /// </summary>
    /// <param name="user">Kullanıcı entity'si.</param>
    /// <param name="rolesToAdd">Eklenecek roller.</param>
    /// <returns>Ekleme işlemi sonuçları.</returns>
    private async Task<List<RoleOperationResult>> AddRolesToUserAsync(User user, List<IdentityRole> rolesToAdd)
    {
        var results = new List<RoleOperationResult>();

        foreach (var role in rolesToAdd)
        {
            var result = await _userManager.AddToRoleAsync(user, role.Name!);
            results.Add(new RoleOperationResult
            {
                RoleId = role.Id,
                RoleName = role.Name!,
                IsSuccess = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        return results;
    }

    /// <summary>
    /// Güncelleme işlemi sonucu için yanıt DTO'sunu oluşturur.
    /// </summary>
    /// <param name="user">Kullanıcı entity'si.</param>
    /// <param name="addResults">Ekleme işlemi sonuçları.</param>
    /// <param name="removeResults">Kaldırma işlemi sonuçları.</param>
    /// <param name="currentRoles">Güncel rol listesi.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private static UpdateUserRolesResponseDto CreateResponseDto(
        User user,
        List<RoleOperationResult> addResults,
        List<RoleOperationResult> removeResults,
        IList<string> currentRoles)
    {
        return new UpdateUserRolesResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName!,
            AddedRoles = addResults,
            RemovedRoles = removeResults,
            CurrentRoles = currentRoles.ToList(),
            Message = "Kullanıcı rolleri başarıyla güncellendi.",
            IsSuccess = addResults.All(r => r.IsSuccess) && removeResults.All(r => r.IsSuccess)
        };
    }

    #endregion
}

#endregion