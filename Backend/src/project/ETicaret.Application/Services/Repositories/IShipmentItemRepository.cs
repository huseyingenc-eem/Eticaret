using Core.Application.Abstractions.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IShipmentItemRepository :IRepository<ShipmentItem, Guid>
{

}