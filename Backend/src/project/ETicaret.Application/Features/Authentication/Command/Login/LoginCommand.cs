using Core.Application.Behaviors.Authorization;
using Core.Application.Common.Constants;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Services.JwtServices;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.Authentication.Command.Login;

public class LoginCommand : IRequest<AccessTokenDto>, IPublicRequest
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
                throw new NotFoundException(
                    message: $"User with email '{request.Email}' was not found.",
                    userFriendlyMessage: "Girdiğiniz email adresine kayıtlı bir kullanıcı bulunamadı. Lütfen email adresinizi kontrol ediniz veya kayıt olunuz.",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "USER_NOT_FOUND"),
                    additionalData: new { Email = request.Email }
                );
            }
            var passwordCheck = await _userManager.CheckPasswordAsync(emailUser, request.Password);

            if (passwordCheck is false)
            {
                throw new BusinessException(
                    message: $"Invalid password provided for user '{emailUser.Email}'.",
                    userFriendlyMessage: "Girdiğiniz şifre hatalıdır. Lütfen şifrenizi kontrol ediniz veya şifre sıfırlama işlemini kullanınız.",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "INVALID_PASSWORD"),
                    additionalData: new
                    {
                        Email = emailUser.Email,
                        UserId = emailUser.Id,
                        AttemptTime = DateTime.UtcNow
                    }
                );
            }

            //if (!emailUser.EmailConfirmed)
            //{
            //    throw new BusinessException(
            //        message: $"Email not confirmed for user '{emailUser.Email}'.",
            //        userFriendlyMessage: "Email adresiniz doğrulanmamış. Lütfen email adresinize gönderilen doğrulama linkine tıklayınız.",
            //        errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "EMAIL_NOT_CONFIRMED"),
            //        additionalData: new { Email = emailUser.Email, UserId = emailUser.Id }
            //    );
            //}

            // 4. Kullanıcı hesabının kilitli olup olmadığını kontrol et
            if (await _userManager.IsLockedOutAsync(emailUser))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(emailUser);
                throw new BusinessException(
                    message: $"User account '{emailUser.Email}' is locked out until {lockoutEnd}.",
                    userFriendlyMessage: $"Hesabınız geçici olarak kilitlenmiştir. Lütfen {lockoutEnd?.ToString("dd.MM.yyyy HH:mm")} tarihinden sonra tekrar deneyiniz.",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "ACCOUNT_LOCKED"),
                    additionalData: new
                    {
                        Email = emailUser.Email,
                        UserId = emailUser.Id,
                        LockoutEnd = lockoutEnd
                    }
                );
            }

            try
            {
                AccessTokenDto token = await _jwtService.CreateTokenAsync(emailUser);
                return token;
            }
            catch (Exception ex)
            {
                throw new BusinessException(
                    message: $"Failed to create access token for user '{emailUser.Email}': {ex.Message}",
                    userFriendlyMessage: "Giriş işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "TOKEN_CREATION_FAILED"),
                    additionalData: new { Email = emailUser.Email, UserId = emailUser.Id }
                );
            }
        }
    }
}