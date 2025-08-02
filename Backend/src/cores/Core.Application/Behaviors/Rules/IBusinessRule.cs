namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Basit business rule interface'i.
/// Her rule tek bir sorumluluğa sahiptir.
/// </summary>
/// <typeparam name="TCommand">Kuralın uygulanacağı command türü</typeparam>
public interface IBusinessRule<in TCommand> where TCommand : class
{
    bool ShouldExecute(TCommand command);
    Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
    int Priority => 0;
}