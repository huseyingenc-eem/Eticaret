using Core.Shared.Exceptions;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Commands.Create;

public class UserRolesAddCommand: IRequest<UserRolesAddResponseDto>
{
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;

    public class UserRolesAddCommandHandler : IRequestHandler<UserRolesAddCommand, UserRolesAddResponseDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesAddCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserRolesAddResponseDto> Handle(UserRolesAddCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException($"Kullanıcı bulunamadı (ID: {request.UserId}).");

            IdentityRole? role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
                throw new NotFoundException($"Rol bulunamadı (ID: {request.RoleId}).");

            if (string.IsNullOrEmpty(role.Name))
            {
                throw new BusinessException($"Bulunan rolün geçerli bir adı yok (ID: {request.RoleId}).");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var roleIsExist = userRoles.Any(x=> x==role.Name);

            if (roleIsExist)
                throw new BusinessException($"Kullanıcının '{role.Name}' rolü zaten mevcut.");

            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(user, role.Name);

            if (!addRoleResult.Succeeded)
            {
                var errors = addRoleResult.Errors.Select(x => x.Description).ToList();
                throw new BusinessException(string.Join(Environment.NewLine, errors));
            }

            return new UserRolesAddResponseDto
            {
                UserId = user.Id,
                RoleName = role.Name,
                Message = $"'{role.Name}' rolü kullanıcıya başarıyla eklendi.",
                IsSuccess = true
            };
        }
    }
}
