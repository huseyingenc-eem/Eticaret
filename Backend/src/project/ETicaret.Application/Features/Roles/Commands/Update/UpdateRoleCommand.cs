using MediatR;
using Microsoft.AspNetCore.Identity;
using Core.Application.Common.Exceptions;
using AutoMapper;

namespace ETicaret.Application.Features.Roles.Commands.Update;

public class UpdateRoleCommand : IRequest<UpdatedRoleResponseDto>
{
    public string Id { get; set; } = string.Empty;
    public string NewName { get; set; } = string.Empty;

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, UpdatedRoleResponseDto>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper; 
        }
        public async Task<UpdatedRoleResponseDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            IdentityRole? roleToUpdate = await _roleManager.FindByIdAsync(request.Id);
            if (roleToUpdate == null)
            {
                throw new NotFoundException($"ID'si '{request.Id}' olan rol bulunamadı.");
            }
            if (!string.Equals(roleToUpdate.Name, request.NewName, StringComparison.OrdinalIgnoreCase))
            {
                IdentityRole? existingRoleWithNewName = await _roleManager.FindByNameAsync(request.NewName);
                if (existingRoleWithNewName != null && existingRoleWithNewName.Id != roleToUpdate.Id)
                {
                    throw new BusinessException($"'{request.NewName}' adında başka bir rol zaten mevcut.");
                }
            }

            roleToUpdate.Name = request.NewName;
            roleToUpdate.NormalizedName = request.NewName.ToUpperInvariant();

            IdentityResult result = await _roleManager.UpdateAsync(roleToUpdate);

            if (!result.Succeeded)
            {
                throw new BusinessException($"Rol güncellenemedi: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            
            UpdatedRoleResponseDto response = _mapper.Map<UpdatedRoleResponseDto>(roleToUpdate);
            response.Message = "Rol başarıyla güncellendi.";
            return response;
        }
    }
}