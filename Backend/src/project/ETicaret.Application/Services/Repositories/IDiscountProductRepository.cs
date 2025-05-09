using ETicaret.Domain.Entities;
using System.Linq.Expressions;
using Core.Persistence.Paging;

namespace ETicaret.Application.Services.Repositories;

public interface IDiscountProductRepository
{
    Task<DiscountProduct?> GetAsync(Expression<Func<DiscountProduct, bool>> filter, bool enableTracking = true);
    Task<IPaginate<DiscountProduct>> GetListAsync(Expression<Func<DiscountProduct, bool>>? filter = null,
                                                   Func<IQueryable<DiscountProduct>, IOrderedQueryable<DiscountProduct>>? orderBy = null,
                                                   int index = 0, int size = 10, bool enableTracking = true);
    Task AddAsync(DiscountProduct discountProduct);
    Task AddRangeAsync(IEnumerable<DiscountProduct> discountProducts);
    Task DeleteAsync(DiscountProduct discountProduct);
    Task DeleteRangeAsync(IEnumerable<DiscountProduct> discountProducts);
}