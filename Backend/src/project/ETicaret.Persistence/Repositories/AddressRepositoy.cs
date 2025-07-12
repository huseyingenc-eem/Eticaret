using Core.Persistence.Paging;
using Core.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETicaret.Persistence.Repositories;

/// <summary>
/// Address entity'si için IAddressRepository interface'ini uygulayan ve
/// Entity Framework Core kullanarak veri erişim operasyonlarını gerçekleştiren sınıf.
/// </summary>
public class AddressRepository : EfRepositoryBase<Address, Guid, BaseDBContexts>, IAddressRepository
{
    /// <summary>
    /// AddressRepository sınıfının yeni bir örneğini başlatır.
    /// </summary>
    /// <param name="context">Veritabanı context'i.</param>
    public AddressRepository(BaseDBContexts context) : base(context)
    {
    }

    public async Task<IPaginate<Address>> GetListWithUserDetailsAsync(Expression<Func<Address, bool>>? predicate = null, Func<IQueryable<Address>, IOrderedQueryable<Address>>? orderBy = null, int index = 0, int size = 10, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        // base.GetListAsync'i çağırırken EF Core'a özgü Include'u burada yapın.
        // Bu, Application katmanının EF Core detaylarını bilmesini engeller.
        return await base.GetListAsync(
            filter: predicate,
            orderBy: orderBy,
            include: query => query.Include(address => address.User), // EF Core Include burada kapsüllendi
            index: index,
            size: size,
            enableTracking: enableTracking,
            cancellationToken: cancellationToken
        );
    }
}