using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IProductRepository : IAsyncRepository<Product,Guid> , IRepository<Product, Guid>
{

}
