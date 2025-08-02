using MediatR;

namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Basit business rules behavior
/// </summary>
public class BusinessRulesBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    
    private readonly IRuleExecutor _ruleExecutor;

    public BusinessRulesBehavior(IRuleExecutor ruleExecutor)
    {
        _ruleExecutor = ruleExecutor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. Business rules'ları otomatik çalıştır
        await _ruleExecutor.ExecuteRulesAsync(request, cancellationToken);

        // 2. Handler'ı çalıştır
        return await next();
    }
}