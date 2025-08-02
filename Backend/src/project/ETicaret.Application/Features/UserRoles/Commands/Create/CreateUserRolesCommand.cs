using Core.Application.Common.Exceptions;
using Core.Application.Behaviors.Transactional;
using Core.Application.Behaviors.Caching;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Core.Application.Behaviors.Authorization;

namespace ETicaret.Application.Features.UserRoles.Commands.Create;

/// <summary>
/// Kullanıcıya yeni bir rol atama işlemini temsil eden komut.
/// ITransactionalRequest: Bu işlemin bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: İşlem başarılı olduğunda ilgili önbelleği temizler.
/// </summary>
[DefaultRoles("Admin")]
public class CreateUserRolesCommand : IRequest<CreateUserRolesResponseDto>, 
    ITransactionalRequest, 
    ICacheRemoverRequest

{
    #region Komut Parametreleri

    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    #endregion

    #region Önbellek Ayarları
    /// <summary>
    /// Kullanıcının rol bilgilerini içeren önbelleği temizler.
    /// </summary>
    public string? CacheKey => $"user-roles:{UserId}";

    /// <summary>
    /// Kullanıcı ve roller ile ilgili tüm önbellek gruplarını temizler.
    /// </summary>
    public string? CacheGroupKey => "UserRoles";

    public bool BypassCache => false;
    #endregion

    #region Komut İşleyici
    /// <summary>
    /// CreateUserRolesCommand komutunu işleyen Handler.
    /// </summary>
    public class CreateUserRolesCommandHandler : IRequestHandler<CreateUserRolesCommand, CreateUserRolesResponseDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateUserRolesCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<CreateUserRolesResponseDto> Handle(CreateUserRolesCommand request, CancellationToken cancellationToken)
        {
            // 1. Parametreleri doğrula
            ValidateRequest(request);

            // 2. Kullanıcıyı getir ve doğrula
            User user = await GetAndValidateUserAsync(request.UserId);

            // 3. Rolü getir ve doğrula
            IdentityRole role = await GetAndValidateRoleAsync(request.RoleId);

            // 4. Kullanıcının bu role sahip olup olmadığını kontrol et
            await ValidateUserDoesNotHaveRoleAsync(user, role.Name!);

            // 5. Rolü kullanıcıya ata
            await AssignRoleToUserAsync(user, role.Name!);

            // 6. Başarı yanıtını oluştur ve döndür
            return CreateSuccessResponse(user, role);
        }

        #region Yardımcı Metotlar

        /// <summary>
        /// İstek parametrelerinin geçerliliğini kontrol eder.
        /// </summary>
        private static void ValidateRequest(CreateUserRolesCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                throw new BusinessException(
                    message: "User ID cannot be null or empty.",
                    userFriendlyMessage: "Kullanıcı ID'si boş olamaz.",
                    errorCode: "INVALID_USER_ID"
                );
            }

            if (string.IsNullOrWhiteSpace(request.RoleId))
            {
                throw new BusinessException(
                    message: "Role ID cannot be null or empty.",
                    userFriendlyMessage: "Rol ID'si boş olamaz.",
                    errorCode: "INVALID_ROLE_ID"
                );
            }
        }

        /// <summary>
        /// Kullanıcıyı getirir ve varlığını doğrular.
        /// </summary>
        private async Task<User> GetAndValidateUserAsync(string userId)
        {
            User? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException(
                    message: $"User with ID '{userId}' was not found.",
                    userFriendlyMessage: $"Kullanıcı bulunamadı (ID: {userId}).",
                    errorCode: "USER_NOT_FOUND"
                );
            }
            return user;
        }

        /// <summary>
        /// Rolü getirir ve varlığını doğrular.
        /// </summary>
        private async Task<IdentityRole> GetAndValidateRoleAsync(string roleId)
        {
            IdentityRole? role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                throw new NotFoundException(
                    message: $"Role with ID '{roleId}' was not found.",
                    userFriendlyMessage: $"Rol bulunamadı (ID: {roleId}).",
                    errorCode: "ROLE_NOT_FOUND"
                );
            }

            if (string.IsNullOrEmpty(role.Name))
            {
                throw new BusinessException(
                    message: $"Role with ID '{roleId}' has an invalid name.",
                    userFriendlyMessage: $"Bulunan rolün geçerli bir adı yok (ID: {roleId}).",
                    errorCode: "INVALID_ROLE_NAME"
                );
            }

            return role;
        }

        /// <summary>
        /// Kullanıcının belirtilen role sahip olmadığını doğrular.
        /// </summary>
        private async Task ValidateUserDoesNotHaveRoleAsync(User user, string roleName)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            bool roleExists = userRoles.Any(x => string.Equals(x, roleName, StringComparison.OrdinalIgnoreCase));

            if (roleExists)
            {
                throw new BusinessException(
                    message: $"User '{user.UserName}' already has the role '{roleName}'.",
                    userFriendlyMessage: $"Kullanıcının '{roleName}' rolü zaten mevcut.",
                    errorCode: "USER_ALREADY_HAS_ROLE"
                );
            }
        }

        /// <summary>
        /// Kullanıcıya rolü atar.
        /// </summary>
        private async Task AssignRoleToUserAsync(User user, string roleName)
        {
            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!addRoleResult.Succeeded)
            {
                var errors = addRoleResult.Errors.Select(x => x.Description).ToList();
                throw new BusinessException(
                    message: $"Failed to assign role '{roleName}' to user '{user.UserName}'. Errors: {string.Join(", ", errors)}",
                    userFriendlyMessage: $"Rol atama işlemi başarısız oldu: {string.Join(Environment.NewLine, errors)}",
                    errorCode: "ROLE_ASSIGNMENT_FAILED"
                );
            }
        }

        /// <summary>
        /// Başarılı işlem için yanıt DTO'sunu oluşturur.
        /// </summary>
        private static CreateUserRolesResponseDto CreateSuccessResponse(User user, IdentityRole role)
        {
            return new CreateUserRolesResponseDto
            {
                UserId = user.Id,
                RoleName = role.Name!,
                Message = $"'{role.Name}' rolü kullanıcıya başarıyla eklendi.",
                IsSuccess = true
            };
        }

        #endregion
    }
    #endregion
}