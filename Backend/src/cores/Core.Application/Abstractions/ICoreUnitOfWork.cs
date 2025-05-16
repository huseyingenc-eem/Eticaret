namespace Core.Application.Abstractions;

/// <summary>
/// Temel Unit of Work işlemlerini tanımlayan arayüz.
/// Bu arayüz, Core.Application gibi temel katmanların,
/// uygulamanın ana UnitOfWork'ünün tüm detaylarını bilmeden
/// işlemleri tamamlayabilmesi (commit edebilmesi) için kullanılır.
/// </summary>
public interface ICoreUnitOfWork : IAsyncDisposable
{
    #region Kaydetme İşlemi
    /// <summary>
    /// Bu Unit of Work kapsamında yapılan tüm değişiklikleri veritabanına asenkron olarak kaydeder.
    /// </summary>
    /// <param name="cancellationToken">İşlemin iptal edilip edilemeyeceğini belirten bir token.</param>
    /// <returns>Veritabanında etkilenen satır sayısını içeren bir görev.</returns>
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    #endregion
}
