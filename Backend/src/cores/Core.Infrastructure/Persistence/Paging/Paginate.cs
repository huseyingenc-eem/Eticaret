using Core.Application.Abstractions.Paging;

namespace Core.Infrastructure.Persistence.Paging;

public class Paginate<T> : IPaginate<T>
{
    public int Index { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }
    public int Pages { get; set; }
    public IList<T> Items { get; set; }
    public bool HasPrevious => Index > 0;
    public bool HasNext => Index + 1 < Pages;
    public Paginate()
    {
        Items = Array.Empty<T>();
    }

    internal Paginate(IEnumerable<T> items, int index, int size, int count)
    {
        Items = items.ToList();
        Index = index;
        Size = size;
        Count = count;
        Pages = (int)Math.Ceiling(count / (double)size);
    }
    
}
