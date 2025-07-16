using Core.Application.Common.Exceptions;
using Core.Shared.Logging.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using Core.Application.Abstractions.Services;

namespace Core.Application.Behaviors.Logging;

/// <summary>
/// MediatR pipeline'ında loglama işlemlerini gerçekleştiren davranış (behavior).
/// ILoggableRequest arayüzünü implemente eden istekleri loglar.
/// </summary>
/// <typeparam name="TRequest">İşlenecek MediatR isteğinin tipi.</typeparam>
/// <typeparam name="TResponse">MediatR isteğinin dönüş tipi.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ILoggableRequest
{
    private readonly ILoggerService _loggerService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public LoggingBehavior(ILoggerService loggerService, IHttpContextAccessor httpContextAccessor)
    {
        _loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// MediatR isteğini işler ve loglama yapar.
    /// </summary>
    /// <param name="request">İşlenecek MediatR isteği.</param>
    /// <param name="next">Pipeline'daki bir sonraki davranışı temsil eden delege.</param>
    /// <param name="cancellationToken">İşlemin iptal edilip edilemeyeceğini belirten bir token.</param>
    /// <returns>İstek işlendikten sonraki yanıtı içeren bir görev.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userPrincipal = httpContext?.User;

        List<LogParameter> logParameters = new List<LogParameter>()
        {
            new LogParameter(){ Type = request.GetType().Name, Value = request}
        };

        var userInfo = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == "Email").Value ?? "?";


        LogDetail logDetail = new()
        {
            ClassFullName = next.Method.DeclaringType?.FullName, 
            MethodName = next.Method.Name,
            Parameters = logParameters,
            User = userPrincipal?.Identity?.Name ?? "Anonymous", 
            UserId = userPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, 
            RequestPath = httpContext?.Request.Path.ToString(),
            RequestMethod = httpContext?.Request.Method,
            ClientIpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
            LogTime = DateTime.UtcNow
        };

        try
        {
            _loggerService.Info(JsonSerializer.Serialize(logDetail));

            var response = await next();

            logDetail.Response = response;
            _loggerService.Info(JsonSerializer.Serialize(logDetail));

            return response;
        }
        catch (Exception ex)
        {
            logDetail.ExceptionType = ex.GetType().FullName;
            logDetail.ExceptionMessage = ex.Message;
            logDetail.ExceptionStackTrace = ex.StackTrace;
            logDetail.ExceptionDetails = ex.ToString(); 

            if (ex is FluentValidationException validationEx)
            {
                // Validasyon hatalarını daha okunabilir bir formatta logla
                logDetail.AdditionalExceptionData = validationEx.Errors
                    .GroupBy(validationErrorModel => validationErrorModel.Property)
                    .ToDictionary(
                        group => group.Key,
                        group => group.SelectMany(validationErrorModel => validationErrorModel.Errors).ToArray() 
                    );
            }

            _loggerService.Error(JsonSerializer.Serialize(logDetail, GetJsonSerializerOptions()));
            throw;
        }
    }
    private JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = false, 
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }
}