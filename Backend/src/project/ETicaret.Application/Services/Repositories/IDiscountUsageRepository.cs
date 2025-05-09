using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IDiscountUsageRepository : IAsyncRepository<DiscountUsage, Guid>, IRepository<DiscountUsage, Guid>
{

}