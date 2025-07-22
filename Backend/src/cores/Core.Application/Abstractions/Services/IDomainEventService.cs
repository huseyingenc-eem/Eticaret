// Konum: cores/Core.Application/Abstractions/Services/IDomainEventService.cs
using Core.Domain.Entities;

namespace Core.Application.Abstractions.Services;

/// <summary>
/// Domain varlıkları tarafından tetiklenen domain olaylarını
/// ilgili handler'lara yayınlamak için kullanılan servis sözleşmesi.
/// </summary>
public interface IDomainEventService
{
    /// <summary>
    /// Verilen varlık listesindeki tüm domain olaylarını asenkron olarak yayınlar.
    /// </summary>
    /// <param name="entitiesWithEvents">Domain olayları içeren varlıkların listesi.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    Task PublishEventsAsync(IEnumerable<IEntity<object>> entitiesWithEvents, CancellationToken cancellationToken = default);
}