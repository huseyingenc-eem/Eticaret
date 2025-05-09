using Core.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

// Review için Repository
public class ReviewRepository : EfRepositoryBase<Review, Guid, BaseDBContexts>, IReviewRepository
{
    public ReviewRepository(BaseDBContexts context) : base(context)
    {
    }
    // IReviewRepository'e özel metot implementasyonları buraya eklenebilir.
}
