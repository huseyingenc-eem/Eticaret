using MediatR;

namespace Core.Application.Abstractions.Messaging;

/// <summary>
/// Bir IQuery'yi işleyen ve bir yanıt döndüren "Handler"ı temsil eder.
/// </summary>
/// <typeparam name="TQuery">İşlenecek sorgunun türü.</typeparam>
/// <typeparam name="TResponse">Döndürülecek yanıtın türü.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>{}