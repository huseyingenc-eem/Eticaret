using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IReviewRepository : IAsyncRepository<Review, Guid>, IRepository<Review, Guid>
{

}