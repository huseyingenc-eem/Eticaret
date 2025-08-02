using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Commands.Delete;

#region Komut Sınıfı (Command Class)

[DefaultRoles("Admin")]
public class DeleteUserRoleCommand : IRequest<DeleteUserRoleResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    #region Komut Parametreleri (Command Parameters)
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;

    #endregion

    #region Önbellek Ayarları (Cache Settings)

    public string? CacheKey => $"user-roles:{UserId}";
    public bool BypassCache => false;
    public string? CacheGroupKey => "UserRoles";

    #endregion
}

#endregion

#region Komut İşleyici (Command Handler)
public class DeleteUserRoleCommandHandler : IRequestHandler<DeleteUserRoleCommand, DeleteUserRoleResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DeleteUserRoleCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<DeleteUserRoleResponseDto> Handle(DeleteUserRoleCommand request, CancellationToken cancellationToken)
    {
        User user = await _userManager.FindByIdAsync(request.UserId)
                    ?? throw new NotFoundException(
                        message: $"User with ID '{request.UserId}' was not found.",
                        userFriendlyMessage: $"Kullanıcı bulunamadı (ID: {request.UserId}).",
                        errorCode: "USER_NOT_FOUND"
                    );

        IdentityRole role = await _roleManager.FindByIdAsync(request.RoleId)
                            ?? throw new NotFoundException(
                                message: $"Role with ID '{request.RoleId}' was not found.",
                                userFriendlyMessage: $"Rol bulunamadı (ID: {request.RoleId}).",
                                errorCode: "ROLE_NOT_FOUND"
                            );

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

        var remainingRoles = await _userManager.GetRolesAsync(user);

        return new DeleteUserRoleResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName!,
            RemovedRoleId = role.Id,
            RemovedRoleName = role.Name!,
            RemainingRoles = remainingRoles.ToList(),
            Message = $"'{role.Name}' rolü kullanıcıdan başarıyla kaldırıldı.",
            IsSuccess = true
        };
    }
}

#endregion