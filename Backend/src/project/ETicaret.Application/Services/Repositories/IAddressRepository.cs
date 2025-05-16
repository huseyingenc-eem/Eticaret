using Core.Persistence.Paging;
using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;
using System.Linq.Expressions;

namespace ETicaret.Application.Services.Repositories;

/// <summary>
/// Address entity'si için veri erişim operasyonlarını tanımlayan interface.
/// Temel CRUD operasyonları için IAsyncRepository ve IRepository'den kalıtım alır.
/// </summary>
public interface IAddressRepository : IAsyncRepository<Address, int>, IRepository<Address, int>
{
    Task<IPaginate<Address>> GetListWithUserDetailsAsync(
            Expression<Func<Address, bool>>? predicate = null,
            Func<IQueryable<Address>, IOrderedQueryable<Address>>? orderBy = null,
            int index = 0,
            int size = 10,
            bool enableTracking = true,
            CancellationToken cancellationToken = default
        );
}