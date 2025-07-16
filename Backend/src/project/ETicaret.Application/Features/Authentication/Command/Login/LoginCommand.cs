using Core.Shared.Exceptions;
using ETicaret.Application.Services.JwtServices;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.Authentication.Command.Login;


public class LoginCommand : IRequest<AccessTokenDto>
{
    public string? Email { get; set; }
    public string? Password { get; set; }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AccessTokenDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(UserManager<User> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<AccessTokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var emailUser = await _userManager.FindByEmailAsync(request.Email);

            if (emailUser is null)
            {
                throw new NotFoundException("İlgili email e göre kullanıcı bulunamadı.");
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(emailUser,request.Password);

            if (passwordCheck is false)
            {
                throw new BusinessException("Parola Yanlış");
            }

            AccessTokenDto token = await _jwtService.CreateTokenAsync(emailUser);
            return token;

        }
    }
}

