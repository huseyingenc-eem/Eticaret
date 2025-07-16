namespace Core.Application.Common.Results;

using Core.Application.Abstractions.Paging;

/// <summary>
/// IPaginate arayüzünün somut implementasyonu. Sayfalanmış sorgu sonuçlarını tutar.
/// </summary>
public class PagedResult<T> : IPaginate<T>
{
    public int From => Index * Size;
    public int Index { get; }
    public int Size { get; }
    public int Count { get; }
    public int Pages { get; }
    public IList<T> Items { get; }
    public bool HasPrevious => Index > 0;
    public bool HasNext => Index + 1 < Pages;

    public PagedResult(IList<T> items, int count, int index, int size)
    {
        Index = index;
        Size = size;
        Count = count;
        Pages = (int)Math.Ceiling(count / (double)size);
        Items = items;
    }
}