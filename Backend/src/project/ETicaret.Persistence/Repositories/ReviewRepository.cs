using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public class ReviewRepository : EfRepositoryBase<Review, Guid, BaseDBContexts>, IReviewRepository
{
    public ReviewRepository(BaseDBContexts context) : base(context) { }
}