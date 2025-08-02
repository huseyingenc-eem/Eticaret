using Core.Application.Behaviors.RequestInfo;
using Core.Application.Common.Exceptions;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace ETicaret.Application.Features.UserRoles.Queries.UserWithRoles;

public class UserWithRolesQuery : IRequest<UserWithRolesResponseDto>, IRequestInfoRequest
{
    [JsonIgnore]
    public string UserId { get; set; }
    public class UserWithRolesQueryHandler : IRequestHandler<UserWithRolesQuery, UserWithRolesResponseDto>
    {
        private readonly UserManager<User> _userManager;

        public UserWithRolesQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserWithRolesResponseDto> Handle(UserWithRolesQuery request, CancellationToken cancellationToken)
        {
            User user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }
            var roles = await _userManager.GetRolesAsync(user);

            UserWithRolesResponseDto responseDto = new()
            {
                City=user.City,
                Email=user.Email,
                Roles=roles,
                UserName=user.UserName
            };

            return responseDto;
        }
    }
}
