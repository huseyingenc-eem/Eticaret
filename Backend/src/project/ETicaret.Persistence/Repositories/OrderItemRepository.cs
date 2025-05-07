using Core.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public sealed class OrderItemRepository : EfRepositoryBase<OrderItem, int, BaseDBContexts>, IOrderItemRepository
{
    public OrderItemRepository(BaseDBContexts context) : base(context)
    {
    }
}
