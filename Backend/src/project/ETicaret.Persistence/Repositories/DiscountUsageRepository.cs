using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class DiscountUsageRepository : EfRepositoryBase<DiscountUsage, Guid, BaseDBContexts>, IDiscountUsageRepository
{
    public DiscountUsageRepository(BaseDBContexts context) : base(context) { }
}