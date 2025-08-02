using Core.Application.Common.Exceptions;
using Core.Application.Common.Constants;
using ETicaret.Application.Services.JwtServices;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Core.Application.Behaviors.Authorization;

namespace ETicaret.Application.Features.Authentication.Command.Register;

public class RegisterCommand : IRequest<AccessTokenDto>, IPublicRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AccessTokenDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;

        public RegisterCommandHandler(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        public async Task<AccessTokenDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // 1. E-posta adresinin daha önce kullanılıp kullanılmadığını kontrol et
            await CheckEmailUniqueAsync(request.Email);

            // 2. Kullanıcı adının daha önce kullanılıp kullanılmadığını kontrol et
            await CheckUsernameUniqueAsync(request.Email);

            // 3. Yeni kullanıcı oluştur
            User newUser = await CreateUserAsync(request, cancellationToken);

            // 4. Varsayılan rolü ata
            await AssignDefaultRoleAsync(newUser);

            // 5. Token oluştur ve döndür
            try
            {
                AccessTokenDto token = await _jwtService.CreateTokenAsync(newUser);
                return token;
            }
            catch (Exception ex)
            {
                throw new BusinessException(
                    message: $"Failed to create access token for user '{newUser.Email}': {ex.Message}",
                    userFriendlyMessage: "Kayıt işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "TOKEN_CREATION_FAILED"),
                    additionalData: new { Email = newUser.Email, UserId = newUser.Id }
                );
            }
        }

        #region Helper Methods

        /// <summary>
        /// E-posta adresinin benzersiz olduğunu kontrol eder.
        /// </summary>
        /// <param name="email">Kontrol edilecek e-posta adresi.</param>
        /// <exception cref="BusinessException">E-posta zaten kullanımdaysa fırlatılır.</exception>
        private async Task CheckEmailUniqueAsync(string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new BusinessException(
                    message: $"Email address '{email}' is already registered.",
                    userFriendlyMessage: "Bu e-posta adresi zaten kayıtlı. Lütfen farklı bir e-posta adresi kullanınız veya giriş yapmayı deneyin.",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "EMAIL_ALREADY_EXISTS"),
                    additionalData: new { Email = email, ExistingUserId = existingUser.Id }
                );
            }
        }

        /// <summary>
        /// Kullanıcı adının benzersiz olduğunu kontrol eder.
        /// </summary>
        /// <param name="username">Kontrol edilecek kullanıcı adı.</param>
        /// <exception cref="BusinessException">Kullanıcı adı zaten kullanımdaysa fırlatılır.</exception>
        private async Task CheckUsernameUniqueAsync(string username)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                throw new BusinessException(
                    message: $"Username '{username}' is already taken.",
                    userFriendlyMessage: "Bu kullanıcı adı zaten kullanılıyor. Lütfen farklı bir kullanıcı adı seçiniz.",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "USERNAME_ALREADY_EXISTS"),
                    additionalData: new { Username = username, ExistingUserId = existingUser.Id }
                );
            }
        }

        /// <summary>
        /// Yeni kullanıcı hesabı oluşturur.
        /// </summary>
        /// <param name="request">Kayıt komutu.</param>
        /// <param name="cancellationToken">İptal token'ı.</param>
        /// <returns>Oluşturulan kullanıcı entity'si.</returns>
        /// <exception cref="BusinessException">Kullanıcı oluşturma işlemi başarısızsa fırlatılır.</exception>
        private async Task<User> CreateUserAsync(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                City = request.City?.Trim(),
                UserName = request.Email.Trim().ToLowerInvariant(),
                Email = request.Email.Trim().ToLowerInvariant(),
                EmailConfirmed = false,
                LockoutEnabled = true,
                TwoFactorEnabled = false
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                var errorMessage = string.Join(Environment.NewLine, errors);

                throw new BusinessException(
                    message: $"Failed to create user account for '{request.Email}': {errorMessage}",
                    userFriendlyMessage: $"Hesap oluşturma işlemi başarısız oldu: {errorMessage}",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "USER_CREATION_FAILED"),
                    additionalData: new
                    {
                        Email = request.Email,
                        Errors = errors,
                        ValidationErrors = result.Errors.Select(e => new { Code = e.Code, Description = e.Description }).ToList()
                    }
                );
            }

            return user;
        }

        /// <summary>
        /// Kullanıcıya varsayılan "User" rolünü atar.
        /// </summary>
        /// <param name="user">Rol atanacak kullanıcı.</param>
        /// <exception cref="BusinessException">Rol atama işlemi başarısızsa fırlatılır.</exception>
        private async Task AssignDefaultRoleAsync(User user)
        {
            const string defaultRole = "User";

            // Rolün mevcut olduğunu kontrol et
            var roleExists = await _roleManager.RoleExistsAsync(defaultRole);
            if (!roleExists)
            {
                throw new BusinessException(
                    message: $"Default role '{defaultRole}' does not exist in the system.",
                    userFriendlyMessage: "Sistem yapılandırma hatası oluştu. Lütfen sistem yöneticisi ile iletişime geçiniz.",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "DEFAULT_ROLE_NOT_FOUND"),
                    additionalData: new { RoleName = defaultRole, UserId = user.Id }
                );
            }

            // Rolü kullanıcıya ata
            var roleResult = await _userManager.AddToRoleAsync(user, defaultRole);
            if (!roleResult.Succeeded)
            {
                var roleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                throw new BusinessException(
                    message: $"Failed to assign default role '{defaultRole}' to user '{user.Email}': {string.Join(", ", roleErrors)}",
                    userFriendlyMessage: "Kullanıcı rolü atama işlemi başarısız oldu. Lütfen sistem yöneticisi ile iletişime geçiniz.",
                    errorCode: ApplicationErrorCodes.CreateErrorCode("AUTHENTICATION", "DEFAULT_ROLE_ASSIGNMENT_FAILED"),
                    additionalData: new
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        RoleName = defaultRole,
                        Errors = roleErrors
                    }
                );
            }
        }

        #endregion
    }
}