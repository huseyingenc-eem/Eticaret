using Core.Application.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Application.Behaviors.RequestInfo;

/// <summary>
/// MediatR pipeline'ına dahil olarak, BaseRequest'ten türeyen tüm isteklere
/// HttpContext üzerinden gelen ortak bilgileri (Kullanıcı ID, Culture, CorrelationId) ekleyen davranış.
/// </summary>
public class RequestInfoBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IRequestInfoRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestInfoBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            // Kullanıcı kimliğini claim'lerden al ve isteğe ata.
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                request.UserId = userIdClaim.Value;
            }

            //request.Culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault();
            //if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationIdHeader) && Guid.TryParse(correlationIdHeader, out Guid correlationId))
            //{
            //    request.CorrelationId = correlationId;
            //}
        }
        return await next();
    }
}