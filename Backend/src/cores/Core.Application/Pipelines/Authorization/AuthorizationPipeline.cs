using Core.CrossCuttingConcerns.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Core.CrossCuttingConcerns.Logger;

namespace Core.Application.Pipelines.Authorization;
public class AuthorizationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILoggerService _loggerService;

    public AuthorizationPipeline(IHttpContextAccessor httpContextAccessor, ILoggerService loggerService)
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
        _loggerService.Info("AuthorizationPipeline Handle metodu başladı."); //Log ekleme

        var httpContext = _httpContextAccessor.HttpContext;

        _loggerService.Info("Token kontrol ediliyor"); //Log ekleme
        if (!httpContext.User.Identity.IsAuthenticated)
        {
            _loggerService.Error("Token geçersiz."); //Log ekleme
            throw new AuthorizationException("Yetkiniz yok.");
        }

        var userRoles = httpContext.User.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList();
        _loggerService.Info("Kullanıcının rolleri bulundu.");
        if (request is IRoleExists)
        {
            _loggerService.Info("Kullanıcının rolleri yetkilerle karşılaştırılıyor.");
            if (!userRoles.Any(x => ((IRoleExists)request).Roles.Contains(x)))
            {
                _loggerService.Error("Yetki yetersiz.");
                throw new AuthorizationException("Yetkiniz yok.");
            }
        }

        _loggerService.Info("AuthorizationPipeline Handle metodu bitti.");

        return await next();
    }
}
