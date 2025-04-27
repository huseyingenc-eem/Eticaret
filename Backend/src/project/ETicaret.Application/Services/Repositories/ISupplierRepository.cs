using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface ISupplierRepository : IAsyncRepository<Supplier,int> , IRepository<Supplier, int>
{

}
