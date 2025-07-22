using Core.Application.Abstractions.Paging;
using Core.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence.Extensions;

public static class EfQueryableExtensions
{
    public static async Task<IPaginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int index, int size, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip(index * size).Take(size).ToListAsync(cancellationToken);

        // Paginate<T> yerine doğrudan PagedResult<T> kullanılıyor.
        return new PagedResult<T>(items, count, index, size);
    }
}