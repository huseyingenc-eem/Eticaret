using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

// Wishlist için Repository
public class WishlistRepository : EfRepositoryBase<Wishlist, Guid, BaseDBContexts>, IWishlistRepository
{
    public WishlistRepository(BaseDBContexts context) : base(context)
    {
    }
    // IWishlistRepository'e özel metot implementasyonları buraya eklenebilir.
}
