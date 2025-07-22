using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

public sealed class CategoryRepository : EfRepositoryBase<Category, int, BaseDBContexts>, ICategoryRepository
{
    public CategoryRepository(BaseDBContexts context) : base(context){}

    public Task<Category?> GetByNameAsync(string name, bool exactMatch = true, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Category>> GetCategoryTreeAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Category?> GetCategoryWithProductsAsync(int categoryId, bool includeInactiveProducts = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Category>> GetChildCategoriesAsync(int parentId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Category>> GetParentCategoriesChainAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Category>> GetRootCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
