using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;
using Core.Infrastructure.Persistence.Repositories;


namespace ETicaret.Persistence.Repositories;

public class PaymentRepository : EfRepositoryBase<Payment, Guid, BaseDBContexts>, IPaymentRepository
{
    public PaymentRepository(BaseDBContexts context) : base(context)
    {

    }
}