using Core.Application.Abstractions.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;
public interface IOrderRepository : IRepository<Order, Guid> {}