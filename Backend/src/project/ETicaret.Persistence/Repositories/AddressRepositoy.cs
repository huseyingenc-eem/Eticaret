using Core.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts; 

namespace ETicaret.Persistence.Repositories;

/// <summary>
/// Address entity'si için IAddressRepository interface'ini uygulayan ve
/// Entity Framework Core kullanarak veri erişim operasyonlarını gerçekleştiren sınıf.
/// </summary>
public class AddressRepository : EfRepositoryBase<Address, int, BaseDBContexts>, IAddressRepository
{
    /// <summary>
    /// AddressRepository sınıfının yeni bir örneğini başlatır.
    /// </summary>
    /// <param name="context">Veritabanı context'i.</param>
    public AddressRepository(BaseDBContexts context) : base(context)
    {
    }

    // IAddressRepository'ye özel metotlar eklenirse, implementasyonları buraya yazılır.
    // Örneğin:
    // public async Task<List<Address>> GetUserShippingAddressesAsync(Guid userId)
    // {
    //     return await GetListAsync(predicate: a => a.UserId == userId && a.IsShippingAddress, enableTracking: false);
    // }
}