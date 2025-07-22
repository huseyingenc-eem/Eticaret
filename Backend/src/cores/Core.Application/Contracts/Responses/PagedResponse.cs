// Konum: cores/Core.Application/Contracts/Responses/PagedResponse.cs
namespace Core.Application.Contracts.Responses;

/// <summary>
/// Sayfalanmış veri listeleri için API yanıt zarfı.
/// Temel yanıt yapısını, sayfalama meta verileriyle genişletir.
/// </summary>
/// <typeparam name="T">Listedeki öğelerin türü.</typeparam>
public class PagedResponse<T> : BaseResponse<IEnumerable<T>>
{
    /// <summary>
    /// Mevcut sayfa numarası (1'den başlar).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Sayfa başına düşen kayıt sayısı.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Toplam sayfa sayısı.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Toplam kayıt sayısı.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Bir önceki sayfanın olup olmadığını gösterir.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Bir sonraki sayfanın olup olmadığını gösterir.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount, Guid correlationId)
        : base(data, correlationId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}