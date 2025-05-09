using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IWishlistItemRepository : IAsyncRepository<WishlistItem, Guid>, IRepository<WishlistItem, Guid>
{

}