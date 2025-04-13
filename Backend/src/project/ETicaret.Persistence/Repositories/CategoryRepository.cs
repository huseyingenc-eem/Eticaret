using Core.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public sealed class CategoryRepository : EfRepositoryBase<Category, int, BaseDBContexts>, ICategoryRepository
{
    public CategoryRepository(BaseDBContexts context) : base(context)
    {

    }
}
