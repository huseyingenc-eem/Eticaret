using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class OrderRepository : EfRepositoryBase<Order, Guid, BaseDBContexts>, IOrderRepository
{
    public OrderRepository(BaseDBContexts context) : base(context) { }
}