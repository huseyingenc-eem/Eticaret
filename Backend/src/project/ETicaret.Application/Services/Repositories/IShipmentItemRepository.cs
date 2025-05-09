using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IShipmentItemRepository : IAsyncRepository<ShipmentItem, Guid>, IRepository<ShipmentItem, Guid>
{

}