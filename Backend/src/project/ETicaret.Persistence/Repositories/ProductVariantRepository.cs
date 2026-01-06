using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class ProductVariantRepository : EfRepositoryBase<ProductVariant, Guid, BaseDBContexts>, IProductVariantRepository
{
    public ProductVariantRepository(BaseDBContexts context) : base(context) { }
}