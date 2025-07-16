using MediatR;

namespace Core.Application.Abstractions.Messaging;

/// <summary>
/// Bir işlem sonucunda bir değer döndüren bir "Query"yi temsil eder.
/// CQRS desenindeki "Query" kısmıdır ve sistemin durumunu değiştirmez.
/// </summary>
/// <typeparam name="TResponse">Sorgu işlendikten sonra dönecek olan yanıtın türü.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>{}