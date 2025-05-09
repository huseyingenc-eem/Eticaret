using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IPaymentRepository : IAsyncRepository<Payment, Guid>, IRepository<Payment, Guid>
{

}