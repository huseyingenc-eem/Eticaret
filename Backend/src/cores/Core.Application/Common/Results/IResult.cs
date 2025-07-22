namespace Core.Application.Common.Results;

/// <summary>
/// Uygulama servislerinden dönen tüm operasyon sonuçları için temel arayüz.
/// İşlemin başarılı olup olmadığını ve bir mesaj içerip içermediğini belirtir.
/// </summary>
public interface IResult
{
    /// <summary>
    /// İşlemin başarılı olup olmadığını gösterir.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// İşlemle ilgili bilgilendirici mesaj. Başarı veya hata detayını içerebilir.
    /// </summary>
    string Message { get; }
}