// C:.
// +---Core.Application
// |   +---Contracts
// |       +---Requests
// |               PagedRequest.cs

namespace Core.Application.Contracts.Requests;

/// <summary>
/// Sayfalama parametrelerini taşıyan temel istek sınıfı.
/// BaseRequest'ten kalıtım alarak CorrelationId gibi tüm ortak özellikleri miras alır.
/// </summary>
public abstract class PagedRequest : BaseRequest
{
    /// <summary>
    /// İstenen sayfa numarası (1'den başlar).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Sayfa başına gösterilecek kayıt sayısı.
    /// </summary>
    public int PageSize { get; set; } = 10;
}