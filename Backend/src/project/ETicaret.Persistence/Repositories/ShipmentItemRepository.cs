using Core.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

// ShipmentItem için Repository
public class ShipmentItemRepository : EfRepositoryBase<ShipmentItem, Guid, BaseDBContexts>, IShipmentItemRepository
{
    public ShipmentItemRepository(BaseDBContexts context) : base(context)
    {
    }
}
