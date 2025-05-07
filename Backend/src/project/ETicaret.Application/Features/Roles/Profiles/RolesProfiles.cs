using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ETicaret.Application.Features.Roles.Commands.Create;
using ETicaret.Application.Features.Roles.Commands.Update;
using ETicaret.Application.Features.Roles.Commands.Delete;

namespace ETicaret.Application.Features.Roles.Profiles;

public class RolesProfile : Profile
{
    public RolesProfile()
    {
        CreateMap<IdentityRole, CreatedRoleResponseDto>()
            .ForMember(dest => dest.Message, opt => opt.Ignore()); 
        
        CreateMap<IdentityRole, UpdatedRoleResponseDto>()
            .ForMember(dest => dest.Message, opt => opt.Ignore());

        CreateMap<IdentityRole, DeletedRoleResponseDto>()
            .ForMember(dest => dest.Message, opt => opt.Ignore());
    }
}
