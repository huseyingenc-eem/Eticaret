using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IProductVariantRepository : IAsyncRepository<ProductVariant,Guid> , IRepository<ProductVariant,Guid>
{

}