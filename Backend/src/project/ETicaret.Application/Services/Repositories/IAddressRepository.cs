using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

/// <summary>
/// Address entity'si için veri erişim operasyonlarını tanımlayan interface.
/// Temel CRUD operasyonları için IAsyncRepository ve IRepository'den kalıtım alır.
/// </summary>
public interface IAddressRepository : IAsyncRepository<Address, int>, IRepository<Address, int>
{
    // Adres'e özel ek metotlar gerekirse buraya tanımlanabilir.
    // Örneğin: Task<List<Address>> GetUserShippingAddressesAsync(Guid userId);
}