using Core.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

// WishlistItem için Repository
public class WishlistItemRepository : EfRepositoryBase<WishlistItem, Guid, BaseDBContexts>, IWishlistItemRepository
{
    public WishlistItemRepository(BaseDBContexts context) : base(context)
    {
    }
    // IWishlistItemRepository'e özel metot implementasyonları buraya eklenebilir.
}
