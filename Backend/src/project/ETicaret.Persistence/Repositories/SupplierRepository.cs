using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

// EfRepositoryBase'den kalıtım alıyoruz: Entity (Supplier), Primary Key (int), DbContext (BaseDBContexts)
// Ayrıca ISupplierRepository interface'ini uyguluyoruz.
public class SupplierRepository : EfRepositoryBase<Supplier, Guid, BaseDBContexts>, ISupplierRepository
{
    public SupplierRepository(BaseDBContexts context) : base(context)
    {

    }
}