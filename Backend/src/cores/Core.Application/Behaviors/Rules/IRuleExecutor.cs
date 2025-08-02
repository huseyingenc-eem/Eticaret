namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Basit rule executor interface'i
/// </summary>
public interface IRuleExecutor
{
    Task ExecuteRulesAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class;
}