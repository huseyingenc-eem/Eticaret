using Core.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class ProductRepository : EfRepositoryBase<Product, int, BaseDBContexts>, IProductRepository
{
    public ProductRepository(BaseDBContexts context) : base(context)
    {

    }
}
