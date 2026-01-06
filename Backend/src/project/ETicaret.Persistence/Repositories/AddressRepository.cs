using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class AddressRepository : EfRepositoryBase<Address, Guid, BaseDBContexts>, IAddressRepository
{
    public AddressRepository(BaseDBContexts context) : base(context)
    {
    }
}