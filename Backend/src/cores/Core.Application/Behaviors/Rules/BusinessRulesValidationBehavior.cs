using MediatR;

namespace Core.Application.Behaviors.Rules;

/// <summary>
/// MediatR pipeline'ında business rule validasyonu yapan behavior.
/// FluentValidation'dan SONRA çalışır ve business logic kontrollerini yapar.
/// </summary>
/// <typeparam name="TRequest">İstek türü - reference type olmalı</typeparam>
/// <typeparam name="TResponse">Yanıt türü</typeparam>
public class BusinessRulesValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IRuleEngine _ruleEngine;

    public BusinessRulesValidationBehavior(IRuleEngine ruleEngine)
    {
        _ruleEngine = ruleEngine;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. Business Rules'ları çalıştır (FluentValidation'dan sonra)
        await _ruleEngine.ValidateAsync(request, cancellationToken);

        // 2. Kurallar geçerse handler'ı çalıştır
        return await next();
    }
}