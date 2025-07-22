using Core.Application.Abstractions.Services;
using Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Services.DomainEvent;

/// <summary>
/// IDomainEventService arayüzünün MediatR kullanarak somut implementasyonu.
/// </summary>
public class DomainEventService : IDomainEventService
{
    private readonly ILogger<DomainEventService> _logger;
    private readonly IPublisher _mediator; // MediatR'ın olay yayınlama arayüzü

    public DomainEventService(ILogger<DomainEventService> logger, IPublisher mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task PublishEventsAsync(IEnumerable<IEntity<object>> entitiesWithEvents, CancellationToken cancellationToken = default)
    {
        // Varlıklardan tüm domain olaylarını topla
        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        _logger.LogInformation("{Count} adet domain olayı bulundu.", domainEvents.Count);

        // Varlıkların içindeki olay listelerini temizle
        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        // Toplanan olayları MediatR aracılığıyla ilgili handler'lara yayınla
        foreach (var domainEvent in domainEvents)
        {
            _logger.LogInformation("{Event} olayı yayınlanıyor.", domainEvent.GetType().Name);
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}