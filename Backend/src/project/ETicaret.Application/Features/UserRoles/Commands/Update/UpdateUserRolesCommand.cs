using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace ETicaret.Application.Features.UserRoles.Commands.Update;

#region Komut Sınıfı (Command Class)

[DefaultRoles("Admin")]
public class UpdateUserRolesCommand : IRequest<UpdateUserRolesResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public string UserId { get; set; } = string.Empty;
    public List<string> RolesToAdd { get; set; } = new();
    public List<string>? RolesToRemove { get; set; } = new();

    public string? CacheKey => $"user-roles:{UserId}";
    public string? CacheGroupKey => "UserRoles";
    public bool BypassCache => false;
}

#endregion

#region Komut İşleyici (Command Handler)

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, UpdateUserRolesResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UpdateUserRolesCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<UpdateUserRolesResponseDto> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        User user = await _userManager.FindByIdAsync(request.UserId)
                    ?? throw new UnreachableException($"UserExistsRule failed to prevent null user with ID {request.UserId}");

        var removeResults = await RemoveRolesFromUserAsync(user, request.RolesToRemove);
        var addResults = await AddRolesToUserAsync(user, request.RolesToAdd);

        var currentRoles = await _userManager.GetRolesAsync(user);

        return new UpdateUserRolesResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName!,
            AddedRoles = addResults,
            RemovedRoles = removeResults,
            CurrentRoles = currentRoles.ToList(),
            Message = "Kullanıcı rolleri başarıyla güncellendi.",
            IsSuccess = addResults.All(r => r.IsSuccess) && (removeResults == null || removeResults.All(r => r.IsSuccess))
        };
    }

    private async Task<List<RoleOperationResult>?> RemoveRolesFromUserAsync(User user, List<string>? roleIdsToRemove)
    {
        if (roleIdsToRemove == null || !roleIdsToRemove.Any()) return new List<RoleOperationResult>();

        var results = new List<RoleOperationResult>();

        foreach (var roleId in roleIdsToRemove.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct())
        {
            var role = await _roleManager.FindByIdAsync(roleId)
                       ?? throw new UnreachableException($"RoleExistsRule failed to prevent null role with ID {roleId}");

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

    private async Task<List<RoleOperationResult>> AddRolesToUserAsync(User user, List<string> roleIdsToAdd)
    {
        if (roleIdsToAdd == null || !roleIdsToAdd.Any()) return new List<RoleOperationResult>();

        var results = new List<RoleOperationResult>();

        foreach (var roleId in roleIdsToAdd.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct())
        {
            var role = await _roleManager.FindByIdAsync(roleId)
                       ?? throw new UnreachableException($"RoleExistsRule failed to prevent null role with ID {roleId}");

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
}

#endregion