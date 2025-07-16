using Core.Application.Abstractions.Paging;
using Core.Infrastructure.Persistence.Paging;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence.Extensions;

public static class EfQueryableExtensions
{
    public static IPaginate<T> ToPaginate<T>(this IQueryable<T> source, int index, int size)
    {
        var count = source.Count();
        var items = source.Skip(index * size).Take(size).ToList();
        return new Paginate<T>(items, index, size, count);
    }

    public static async Task<IPaginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int index, int size, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip(index * size).Take(size).ToListAsync(cancellationToken);
        return new Paginate<T>(items, index, size, count);
    }
}