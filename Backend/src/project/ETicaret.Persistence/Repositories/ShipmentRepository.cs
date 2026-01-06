using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class ShipmentRepository : EfRepositoryBase<Shipment, Guid, BaseDBContexts>, IShipmentRepository
{
    public ShipmentRepository(BaseDBContexts context) : base(context) { }
}