using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Gelişmiş kural motoru implementasyonu.
/// Seçici kural çalıştırma ve performans optimizasyonu sağlar.
/// </summary>
public class RuleEngine : IRuleEngine
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRuleConfigurationService _ruleConfigurationService;
    private readonly ILogger<RuleEngine> _logger;

    public RuleEngine(
        IServiceProvider serviceProvider,
        IRuleConfigurationService ruleConfigurationService,
        ILogger<RuleEngine> logger)
    {
        _serviceProvider = serviceProvider;
        _ruleConfigurationService = ruleConfigurationService;
        _logger = logger;
    }

    public async Task ValidateAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class
    {
        try
        {
            var commandType = typeof(TCommand);
            _logger.LogDebug("Starting rule validation for command: {CommandType}", commandType.Name);

            // Çalıştırılacak kuralları al
            var ruleTypesToExecute = _ruleConfigurationService.GetRulesToExecute<TCommand>();

            if (!ruleTypesToExecute.Any())
            {
                _logger.LogDebug("No rules configured for command: {CommandType}", commandType.Name);
                return;
            }

            // Kuralları oluştur ve sırala
            var rules = CreateAndSortRules<TCommand>(ruleTypesToExecute);

            // Kuralları sırasıyla çalıştır
            foreach (var rule in rules)
            {
                try
                {
                    _logger.LogDebug("Executing rule: {RuleName} for command: {CommandType}",
                        rule.RuleName, commandType.Name);

                    await rule.ExecuteAsync(command, cancellationToken);

                    _logger.LogDebug("Rule {RuleName} executed successfully", rule.RuleName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Rule {RuleName} failed for command {CommandType}",
                        rule.RuleName, commandType.Name);
                    throw;
                }
            }

            _logger.LogDebug("All rules executed successfully for command: {CommandType}", commandType.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rule validation failed for command: {CommandType}", typeof(TCommand).Name);
            throw;
        }
    }

    private IEnumerable<IRule<TCommand>> CreateAndSortRules<TCommand>(IEnumerable<Type> ruleTypes)
        where TCommand : class
    {
        var rules = new List<IRule<TCommand>>();

        foreach (var ruleType in ruleTypes)
        {
            try
            {
                var rule = (IRule<TCommand>)_serviceProvider.GetRequiredService(ruleType);
                rules.Add(rule);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create rule instance: {RuleType}", ruleType.Name);
            }
        }

        // Priority'ye göre sırala (düşük değer = yüksek öncelik)
        return rules.OrderBy(rule => rule.Priority).ToList();
    }
}