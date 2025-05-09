using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IShipmentRepository : IAsyncRepository<Shipment, Guid>, IRepository<Shipment, Guid>
{

}