using Core.CrossCuttingConcerns.Exceptions;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Commands.Create;

public class UserRolesAddCommand: IRequest<string>
{
    public string? UserId { get; set; }
    public string? RoleId { get; set; }

    public class UserRolesAddCommandHandler : IRequestHandler<UserRolesAddCommand, string>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesAddCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> Handle(UserRolesAddCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }

            IdentityRole? role = await _roleManager.FindByIdAsync(request.RoleId);

            if (role is null)
            {
                throw new NotFoundException("Rol Bulunamad.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var roleIsExist = userRoles.Any(x=> x==role.Name);

            if (roleIsExist)
            {
                throw new BusinessException("Kullanıcının bu rolü zaten mevcut");
            }

            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(user, role.Name);

            if (!addRoleResult.Succeeded)
            {
                var errors = addRoleResult.Errors.Select(x=> x.Description).ToList();
                throw new AuthorizationException(errors);
            }

            return "Kullanıcıya Rol eklendi";






        }
    }
}
