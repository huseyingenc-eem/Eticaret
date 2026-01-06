using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class PaymentRepository : EfRepositoryBase<Payment, Guid, BaseDBContexts>, IPaymentRepository
{
    public PaymentRepository(BaseDBContexts context) : base(context) { }
}