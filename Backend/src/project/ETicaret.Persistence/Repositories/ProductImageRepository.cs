using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class ProductImageRepository : EfRepositoryBase<ProductImage, Guid, BaseDBContexts>, IProductImageRepository
{
    public ProductImageRepository(BaseDBContexts context) : base(context)
    {
    }
}
