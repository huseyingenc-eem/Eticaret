using Core.Application.Abstractions.Services;
using Core.Application.Common.Constants;
using Core.Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Application.Behaviors.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizationRuleService _authorizationRuleService;
    private readonly ILoggerService _loggerService;

    public AuthorizationBehavior(
        IHttpContextAccessor httpContextAccessor,
        IAuthorizationRuleService authorizationRuleService,
        ILoggerService loggerService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizationRuleService = authorizationRuleService;
        _loggerService = loggerService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;

        // 1. Dinamik olarak veritabanından/cache'den gerekli rolleri al
        var requiredRoles = await _authorizationRuleService.GetRequiredRolesAsync(requestName);

        // 2. Eğer bu işlem için hiç rol tanımlanmamışsa, herkese açık kabul et ve devam et.
        if (requiredRoles == null || !requiredRoles.Any())
        {
            _loggerService.Info($"'{requestName}' işlemi için rol gereksinimi bulunmuyor. Herkese açık.");
            return await next();
        }

        // 3. Rol gerekiyorsa, kullanıcının giriş yapıp yapmadığını kontrol et.
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            _loggerService.Error($"Yetkisiz erişim denemesi: '{requestName}'. Kullanıcı giriş yapmamış.");
            throw new AuthenticationException(
                        message: "Bu işlemi yapmak için giriş yapmalısınız.",
                        userFriendlyMessage: "Lütfen giriş yapınız.",
                        errorCode: ApplicationErrorCodes.AuthGeneral
                        );
        }

        // 4. Kullanıcının rollerini token'dan al.
        var userRoles = httpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // 5. Kullanıcının rollerinden herhangi biri, gereken rollerden biriyle eşleşiyor mu?
        bool isAuthorized = requiredRoles.Any(requiredRole => userRoles.Contains(requiredRole));

        if (!isAuthorized)
        {
            _loggerService.Error($"Yetki yetersiz: '{requestName}'. Gerekli Roller: [{string.Join(", ", requiredRoles)}], Kullanıcı Rolleri: [{string.Join(", ", userRoles)}]");
            throw new AuthorizationException(
                message: $"Bu işlem için yetkiniz yok. Gerekli roller: {string.Join(", ", requiredRoles)}",
                userFriendlyMessage: "Bu işlem için yetkiniz bulunmuyor.",
                errorCode: ApplicationErrorCodes.AuthGeneral,
                additionalData: new { RequiredRoles = requiredRoles, UserRoles = userRoles }
            );
        }

        _loggerService.Info($"Yetki başarılı: '{requestName}'. Kullanıcı rolleri: [{string.Join(", ", userRoles)}]");
        return await next();
    }
}