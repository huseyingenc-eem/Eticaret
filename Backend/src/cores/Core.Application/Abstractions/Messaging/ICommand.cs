using MediatR;

namespace Core.Application.Abstractions.Messaging;

/// <summary>
/// Bir işlem sonucunda bir değer döndüren bir "Command"i temsil eder.
/// CQRS desenindeki "Command" kısmıdır ve sistemin durumunu değiştirir.
/// </summary>
/// <typeparam name="TResponse">Komut işlendikten sonra dönecek olan yanıtın türü.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>{}

/// <summary>
/// Bir işlem sonucunda geriye bir değer döndürmeyen bir "Command"i temsil eder.
/// </summary>
public interface ICommand : IRequest{}