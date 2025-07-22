using Core.Application.Abstractions.Services;
using Core.Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Application.Behaviors.Authorization;
public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILoggerService _loggerService;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor, ILoggerService loggerService)
    {
        _httpContextAccessor = httpContextAccessor;
        _loggerService = loggerService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not IRoleExists)
        {
            return await next();
        }
        _loggerService.Info("AuthorizationBehavior Handle metodu başladı.");

        var httpContext = _httpContextAccessor.HttpContext;

        _loggerService.Info("Token kontrol ediliyor");
        if (!httpContext.User.Identity.IsAuthenticated)
        {
            _loggerService.Error("Token geçersiz.");
            throw new AuthorizationException("Yetkiniz yok.");
        }

        var userRoles = httpContext.User.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList();
        _loggerService.Info("Kullanıcının rolleri bulundu.");

        _loggerService.Info("Kullanıcının rolleri yetkilerle karşılaştırılıyor.");
        if (!userRoles.Any(x => ((IRoleExists)request).Roles.Contains(x)))
        {
            _loggerService.Error("Yetki yetersiz.");
            throw new AuthorizationException("Yetkiniz yok.");
        }

        _loggerService.Info("AuthorizationBehavior Handle metodu bitti.");

        return await next();
    }
}
