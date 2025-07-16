using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

// ShoppingCart için Repository
public class ShoppingCartRepository : EfRepositoryBase<ShoppingCart, Guid, BaseDBContexts>, IShoppingCartRepository
{
    public ShoppingCartRepository(BaseDBContexts context) : base(context)
    {
    }
    // IShoppingCartRepository'e özel metot implementasyonları buraya eklenebilir.
}
