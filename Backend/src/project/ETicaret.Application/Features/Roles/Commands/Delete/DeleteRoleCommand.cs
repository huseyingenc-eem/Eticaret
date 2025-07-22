using MediatR;
using Microsoft.AspNetCore.Identity;
using Core.Application.Common.Exceptions;
using AutoMapper;

namespace ETicaret.Application.Features.Roles.Commands.Delete;

public class DeleteRoleCommand : IRequest<DeletedRoleResponseDto>
{
    public string Id { get; set; } = string.Empty;

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, DeletedRoleResponseDto>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public DeleteRoleCommandHandler(RoleManager<IdentityRole> roleManager, IMapper mapper) // IMapper enjekte edildi
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<DeletedRoleResponseDto> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            IdentityRole? roleToDelete = await _roleManager.FindByIdAsync(request.Id);
            if (roleToDelete == null)
            {
                throw new NotFoundException($"ID'si '{request.Id}' olan rol bulunamadı.");
            }

            IdentityResult result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new BusinessException($"Rol silinemedi: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            DeletedRoleResponseDto response = _mapper.Map<DeletedRoleResponseDto>(roleToDelete);

            response.Message = "Rol başarıyla silindi.";
            return response;
        }
    }
}