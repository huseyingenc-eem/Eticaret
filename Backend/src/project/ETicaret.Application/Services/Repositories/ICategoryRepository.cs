using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface ICategoryRepository : IAsyncRepository<Category,int> , IRepository<Category,int>
{

}
