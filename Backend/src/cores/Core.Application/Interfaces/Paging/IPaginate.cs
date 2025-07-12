namespace Core.Application.Interfaces.Paging;

public interface IPaginate<T>
{
    /// <summary>
    /// Geçerli sayfa indeksi (genellikle 0'dan başlar).
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Sayfa başına düşen öğe sayısı.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Toplam öğe sayısı.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Toplam sayfa sayısı.
    /// </summary>
    int Pages { get; }

    /// <summary>
    /// Geçerli sayfadaki öğeleri içeren liste.
    /// </summary>
    IList<T> Items { get; }

    /// <summary>
    /// Önceki sayfanın olup olmadığını gösterir.
    /// </summary>
    bool HasPrevious { get; }

    /// <summary>
    /// Sonraki sayfanın olup olmadığını gösterir.
    /// </summary>
    bool HasNext { get; }
}

