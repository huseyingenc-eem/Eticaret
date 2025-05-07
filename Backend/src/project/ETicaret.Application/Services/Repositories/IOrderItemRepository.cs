using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IOrderItemRepository : IAsyncRepository<OrderItem, int> , IRepository<OrderItem, int>
{

}
