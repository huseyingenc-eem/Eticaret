using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class OperationClaimRepository : EfRepositoryBase<OperationClaim, int, BaseDBContexts>, IOperationClaimRepository
{
    public OperationClaimRepository(BaseDBContexts context) : base(context)
    {

    }
}
