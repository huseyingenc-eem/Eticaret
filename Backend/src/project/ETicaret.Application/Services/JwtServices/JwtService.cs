using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ETicaret.Application.Services.JwtServices;


public class JwtService : IJwtService
{
    private readonly UserManager<User> _userManager;

    //ayrı yerden alıyoruz maplediğimiz için
    private readonly CustomTokenOptions _customTokenOptions;

    public JwtService(UserManager<User> userManager, IOptions<CustomTokenOptions> options)
    {
        _userManager = userManager;
        _customTokenOptions = options.Value;
    }

    public async Task<AccessTokenDto> CreateTokenAsync(User user)
    {
        var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOptions.AccessTokenExpiration);
        var symetricKey = GetSecurityKey(_customTokenOptions.SecurityKey);

        SigningCredentials signintCredentials = new SigningCredentials(symetricKey,SecurityAlgorithms.HmacSha512Signature);

        JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: _customTokenOptions.Issuer,
                audience: _customTokenOptions.Audience[0],
                expires: accessTokenExpiration,
                signingCredentials: signintCredentials,
                claims: await GetClaims(user)
            );

        var jwtHandler = new JwtSecurityTokenHandler();

        string token = jwtHandler.WriteToken(jwt);

        AccessTokenDto accessTokenDto = new AccessTokenDto
        {
            Token = token,
            TokenExpiration = accessTokenExpiration
        };

        return accessTokenDto;

    }
    private SecurityKey GetSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
    private  async Task<List<Claim>> GetClaims(User user)
    {
        var claimList = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Count>0)
        {
            claimList.AddRange(roles.Select(x=>new Claim(ClaimTypes.Role,x)));
        }

        return claimList;


    }


}
