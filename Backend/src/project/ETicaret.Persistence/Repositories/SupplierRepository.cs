using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class SupplierRepository : EfRepositoryBase<Supplier, Guid, BaseDBContexts>, ISupplierRepository
{
    public SupplierRepository(BaseDBContexts context) : base(context) { }
}