using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.JwtServices;

public interface IJwtService
{
    Task<AccessTokenDto> CreateTokenAsync(User user);

}
