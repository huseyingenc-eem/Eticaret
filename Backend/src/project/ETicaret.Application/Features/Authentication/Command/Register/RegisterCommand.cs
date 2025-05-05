using Core.CrossCuttingConcerns.Exceptions;
using ETicaret.Application.Services.JwtServices;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.Authentication.Command.Register;

public class RegisterCommand : IRequest<AccessTokenDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? Password { get; set; }


    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AccessTokenDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;

        public RegisterCommandHandler(UserManager<User> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<AccessTokenDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            User user = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                City = request.City,
                UserName = request.UserName,
                Email = request.Email,

            };
            var emailUserCheck = await _userManager.FindByEmailAsync(request.Email);
            if (emailUserCheck is not null)
                throw new BusinessException("Kullanıcı Emaili benzersiz olmalıdır.");

            var userNameCheck = await _userManager.FindByNameAsync(request.UserName);
            if (userNameCheck is not null)
                throw new BusinessException("Bu kullanıcı adı zaten kullanılıyor.");


            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                throw new AuthorizationException(errors);
            }
            AccessTokenDto token = await _jwtService.CreateTokenAsync(user);

            return token;
        }
    }
}
