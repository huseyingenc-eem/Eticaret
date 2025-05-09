using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface IProductImageRepository : IAsyncRepository<ProductImage, Guid> , IRepository<ProductImage,Guid>
{

}