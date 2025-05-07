using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IOperationClaimRepository : IRepository<OperationClaim, int>, IAsyncRepository<OperationClaim, int>
{
}
