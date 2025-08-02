using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Basit rule executor implementasyonu
/// </summary>
public class RuleExecutor : IRuleExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RuleExecutor> _logger;

    public RuleExecutor(IServiceProvider serviceProvider, ILogger<RuleExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ExecuteRulesAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class
    {
        // 1. İlgili command için rule'ları bul
        var rules = _serviceProvider.GetServices<IBusinessRule<TCommand>>();

        if (!rules.Any())
        {
            _logger.LogDebug("No rules found for command: {CommandType}", typeof(TCommand).Name);
            return;
        }

        // 2. ShouldExecute kontrolü ile filtrele ve önceliğe göre sırala
        var applicableRules = rules
            .Where(rule => rule.ShouldExecute(command))
            .OrderBy(rule => rule.Priority)
            .ToList();

        // 3. Rule'ları sırasıyla çalıştır
        foreach (var rule in applicableRules)
        {
            try
            {
                _logger.LogDebug("Executing rule: {RuleType} for command: {CommandType}",
                    rule.GetType().Name, typeof(TCommand).Name);

                await rule.ExecuteAsync(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rule {RuleType} failed for command {CommandType}",
                    rule.GetType().Name, typeof(TCommand).Name);
                throw;
            }
        }
    }
}