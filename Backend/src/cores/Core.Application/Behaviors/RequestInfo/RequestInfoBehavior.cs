using Core.Application.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Application.Behaviors.RequestInfo;

/// <summary>
/// MediatR pipeline'ına dahil olarak, BaseRequest'ten türeyen tüm isteklere
/// HttpContext üzerinden gelen ortak bilgileri (Kullanıcı ID, Culture, CorrelationId) ekleyen davranış.
/// </summary>
/// <typeparam name="TRequest">İşlenecek istek tipi (IRequest'ten türemeli).</typeparam>
/// <typeparam name="TResponse">İstekten dönecek cevap tipi.</typeparam>
public class RequestInfoBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// RequestInfoBehavior sınıfının bir örneğini oluşturur.
    /// </summary>
    /// <param name="httpContextAccessor">Mevcut HTTP isteğine erişim sağlamak için kullanılır.</param>
    public RequestInfoBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// MediatR pipeline'ı tarafından çağrılan ana metot. İsteği işler ve ortak bilgileri ekler.
    /// </summary>
    /// <param name="request">İşlenmekte olan mevcut istek nesnesi.</param>
    /// <param name="next">Pipeline'daki bir sonraki adımı (başka bir behavior veya asıl handler) temsil eden delege.</param>
    /// <param name="cancellationToken">İşlemin iptal edilip edilmediğini kontrol eden token.</param>
    /// <returns>Pipeline'daki bir sonraki adımdan dönen sonuç.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            // Kullanıcı kimliğini claim'lerden al ve isteğe ata.
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                request.AuthenticatedUserId = userId;
            }

            // Culture bilgisini "Accept-Language" header'ından al.
            request.Culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault();

            // Varsa, dış dünyadan gelen X-Correlation-ID'yi kullan, yoksa istekteki mevcut ID kalır.
            if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationIdHeader) && Guid.TryParse(correlationIdHeader, out Guid correlationId))
            {
                request.CorrelationId = correlationId;
            }
        }

        // İsteği pipeline'daki bir sonraki adıma geçir.
        return await next();
    }
}