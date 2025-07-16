using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

// Discount için Repository
public class DiscountRepository : EfRepositoryBase<Discount, Guid, BaseDBContexts>, IDiscountRepository
{
    public DiscountRepository(BaseDBContexts context) : base(context)
    {
    }
    // IDiscountRepository'e özel metot implementasyonları buraya eklenebilir.
}
