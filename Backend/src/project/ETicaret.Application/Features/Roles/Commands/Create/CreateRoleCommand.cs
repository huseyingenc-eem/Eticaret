using MediatR;
using Microsoft.AspNetCore.Identity;
using Core.Application.Common.Exceptions;
using AutoMapper;

namespace ETicaret.Application.Features.Roles.Commands.Create;

public class CreateRoleCommand : IRequest<CreatedRoleResponseDto>
{
    public string Name { get; set; } = string.Empty;
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, CreatedRoleResponseDto>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper; // Eklendi

        public CreateRoleCommandHandler(RoleManager<IdentityRole> roleManager, IMapper mapper) // Mapper enjekte edildi
        {
            _roleManager = roleManager;
            _mapper = mapper; // Eklendi
        }

        public async Task<CreatedRoleResponseDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(request.Name);
            if (roleExists)
            {
                throw new BusinessException($"'{request.Name}' adında bir rol zaten mevcut.");
            }

            IdentityRole newRole = new() { Name = request.Name, NormalizedName = request.Name.ToUpperInvariant() };
            IdentityResult result = await _roleManager.CreateAsync(newRole);

            if (!result.Succeeded)
            {
                throw new BusinessException($"Rol oluşturulamadı: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            var createdIdentityRole = await _roleManager.FindByNameAsync(newRole.Name);

            CreatedRoleResponseDto response = _mapper.Map<CreatedRoleResponseDto>(createdIdentityRole);
            response.Message = "Rol başarıyla oluşturuldu.";
            return response;
        }
    }
}