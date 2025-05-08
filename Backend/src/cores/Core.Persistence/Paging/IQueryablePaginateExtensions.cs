using Microsoft.EntityFrameworkCore;

namespace Core.Persistence.Paging;

public static class IQueryablePaginateExtensions
{
    /// <summary>
    /// Bir IQueryable kaynağını asenkron olarak sayfalar.
    /// </summary>
    /// <typeparam name="T">Kaynak koleksiyondaki öğelerin tipi.</typeparam>
    /// <param name="source">Sayfalanacak IQueryable kaynağı.</param>
    /// <param name="index">İstenen sayfa indeksi (0'dan başlar).</param>
    /// <param name="size">Sayfa başına öğe sayısı.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Sayfalanmış veriyi ve sayfalama bilgilerini içeren bir IPaginate<T> nesnesi.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Index veya size negatifse fırlatılır.</exception>
    public static async Task<IPaginate<T>> ToPaginateAsync<T>(
        this IQueryable<T> source, int index, int size,
        CancellationToken cancellationToken = default)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Sayfa indeksi negatif olamaz.");
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Sayfa boyutu 0'dan büyük olmalıdır.");

        int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await source.Skip(index * size)
                                .Take(size)
                                .ToListAsync(cancellationToken)
                                .ConfigureAwait(false);

        var list = new Paginate<T>(items, index, size, count);

        return list;
    }
    /// <summary>
    /// Bir IQueryable kaynağını senkron olarak sayfalar (Asenkron olmayan senaryolar için).
    /// </summary>
    /// <typeparam name="T">Kaynak koleksiyondaki öğelerin tipi.</typeparam>
    /// <param name="source">Sayfalanacak IQueryable kaynağı.</param>
    /// <param name="index">İstenen sayfa indeksi (0'dan başlar).</param>
    /// <param name="size">Sayfa başına öğe sayısı.</param>
    /// <returns>Sayfalanmış veriyi ve sayfalama bilgilerini içeren bir IPaginate<T> nesnesi.</returns>
    public static IPaginate<T> ToPaginate<T>(this IQueryable<T> source, int index, int size)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Sayfa indeksi negatif olamaz.");
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Sayfa boyutu 0'dan büyük olmalıdır.");

        int count = source.Count();
        var items = source.Skip(index * size).Take(size).ToList();
        var list = new Paginate<T>(items, index, size, count);
        return list;
    }
}
