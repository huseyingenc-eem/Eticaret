using MediatR;

namespace Core.Application.Abstractions.Messaging;

/// <summary>
/// Bir ICommand'i işleyen ve bir yanıt döndüren "Handler"ı temsil eder.
/// </summary>
/// <typeparam name="TCommand">İşlenecek komutun türü.</typeparam>
/// <typeparam name="TResponse">Döndürülecek yanıtın türü.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// Bir ICommand'i işleyen ancak bir yanıt döndürmeyen "Handler"ı temsil eder.
/// </summary>
/// <typeparam name="TCommand">İşlenecek komutun türü.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}