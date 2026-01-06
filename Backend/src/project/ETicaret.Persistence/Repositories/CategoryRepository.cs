using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

/// <summary>
/// Category varlığı için veritabanı işlemlerini gerçekleştiren konkret repository sınıfı.
/// </summary>
public class CategoryRepository : EfRepositoryBase<Category, int, BaseDBContexts>, ICategoryRepository
{
    public CategoryRepository(BaseDBContexts context) : base(context)
    {
    }
}