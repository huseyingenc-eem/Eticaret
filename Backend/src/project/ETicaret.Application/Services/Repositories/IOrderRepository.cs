using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IOrderRepository : IAsyncRepository<Order,Guid> , IRepository<Order,Guid>
{
}
