using Microsoft.EntityFrameworkCore;
using Core.Application.Abstractions.Paging;


namespace Core.Infrastructure.Persistence.Paging;

/// <summary>
/// IQueryable için sayfalama (pagination) genişletme metotları içerir.
/// </summary>
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
        if (size < 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Sayfa boyutu 0'dan büyük olmalıdır.");

        int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);

        List<T> items;
        if (size == 0) 
            items = new List<T>();
        else
            items = await source.Skip(index * size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false);

        return new Paginate<T>
        {
            Index = index,
            Size = size,
            Count = count,
            Items = items,
            Pages = (int)Math.Ceiling(count / (double)Math.Max(1, size)) 
        };
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
        List<T> items;

        if (size == 0)
            items = new List<T>();
        else
            items = source.Skip(index * size).Take(size).ToList();

        // Paginate<T> nesnesini oluştur ve döndür.
        return new Paginate<T>
        {
            Index = index,
            Size = size,
            Count = count,
            Items = items,
            Pages = (int)Math.Ceiling(count / (double)Math.Max(1, size))
        };
    }
}
