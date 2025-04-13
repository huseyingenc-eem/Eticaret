using Core.CrossCuttingConcerns.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.Roles.Commands.Create;

public class RolesAddCommand : IRequest<string>
{
    public string? Name { get; set; }



    public class RolesAddCommandHandler : IRequestHandler<RolesAddCommand, string>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesAddCommandHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<string> Handle(RolesAddCommand request, CancellationToken cancellationToken)
        {
            // ilgili veri tabanında var mı yok mu?
            bool roleIsPresent = await _roleManager.RoleExistsAsync(request.Name);

            if (roleIsPresent )
            {
                throw new BusinessException("Ekleme istediğiniz benzersiz olmalıdır.");
            }

            IdentityRole role = new IdentityRole()
            {
                Name = request.Name
            };
            await _roleManager.CreateAsync(role);

            return "Role Başarıyla Eklendi";
        }
    }
}
