using System.Collections.Concurrent;
using System.Reflection;

namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Kural yapılandırmasını yöneten servis implementasyonu.
/// Thread-safe ve performans odaklı tasarım.
/// </summary>
public class RuleConfigurationService : IRuleConfigurationService
{
    private readonly ConcurrentDictionary<Type, IEnumerable<Type>> _ruleCache = new();
    private readonly IServiceProvider _serviceProvider;

    public RuleConfigurationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<Type> GetRulesToExecute<TCommand>() where TCommand : class
    {
        var commandType = typeof(TCommand);

        return _ruleCache.GetOrAdd(commandType, _ => BuildRuleConfiguration<TCommand>());
    }

    public void CacheRuleConfiguration<TCommand>() where TCommand : class
    {
        var commandType = typeof(TCommand);
        _ruleCache.TryAdd(commandType, BuildRuleConfiguration<TCommand>());
    }

    private IEnumerable<Type> BuildRuleConfiguration<TCommand>() where TCommand : class
    {
        var commandType = typeof(TCommand);
        var attributes = commandType.GetCustomAttributes<RuleConfigurationAttribute>().ToArray();

        // Eğer hiç attribute yoksa, tüm mevcut kuralları çalıştır
        if (!attributes.Any())
        {
            return GetAllAvailableRuleTypes<TCommand>();
        }

        var allAvailableRules = GetAllAvailableRuleTypes<TCommand>().ToHashSet();
        var rulesToExecute = new HashSet<Type>();

        foreach (var attribute in attributes)
        {
            // OnlyRules varsa, sadece bunları çalıştır
            if (attribute.OnlyRules?.Any() == true)
            {
                foreach (var ruleType in attribute.OnlyRules)
                {
                    if (IsValidRuleType<TCommand>(ruleType))
                    {
                        rulesToExecute.Add(ruleType);
                    }
                }
                continue;
            }

            // Include kuralları ekle
            foreach (var ruleType in attribute.IncludeRules)
            {
                if (IsValidRuleType<TCommand>(ruleType))
                {
                    rulesToExecute.Add(ruleType);
                }
            }

            // Exclude kuralları çıkar
            foreach (var ruleType in attribute.ExcludeRules)
            {
                rulesToExecute.Remove(ruleType);
            }
        }

        // Eğer hiç kural seçilmemişse, tüm mevcut kuralları döndür
        return rulesToExecute.Any() ? rulesToExecute : allAvailableRules;
    }

    private IEnumerable<Type> GetAllAvailableRuleTypes<TCommand>() where TCommand : class
    {
        var ruleInterfaceType = typeof(IRule<TCommand>);

        // Assembly'lerden IRule<TCommand> uygulayan tüm türleri bul
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t != null)!;
                }
                catch
                {
                    return Enumerable.Empty<Type>();
                }
            })
            .Where(type => type.IsClass && !type.IsAbstract &&
                          ruleInterfaceType.IsAssignableFrom(type))
            .ToList();
    }

    private static bool IsValidRuleType<TCommand>(Type ruleType) where TCommand : class
    {
        return ruleType.IsClass && !ruleType.IsAbstract &&
               typeof(IRule<TCommand>).IsAssignableFrom(ruleType);
    }
}