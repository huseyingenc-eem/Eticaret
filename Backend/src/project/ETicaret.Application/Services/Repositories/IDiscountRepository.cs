using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IDiscountRepository : IAsyncRepository<Discount, Guid>, IRepository<Discount, Guid>
{

}