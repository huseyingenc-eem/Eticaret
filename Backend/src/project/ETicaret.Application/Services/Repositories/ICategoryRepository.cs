using Core.Application.Abstractions.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

/// <summary>
/// Category entity'si için domain-specific veri erişim operasyonlarını tanımlayan interface.
/// Temel CRUD operasyonları için IAsyncRepository ve IRepository'den kalıtım alır.
/// </summary>
public interface ICategoryRepository : IRepository<Category,int>
{
    /// <summary>
    /// Belirtilen kategorinin tüm alt kategorilerini (aktif/pasif durumuna göre) getirir.
    /// </summary>
    /// <param name="parentId">Ana kategori ID'si</param>
    /// <param name="includeInactive">Pasif kategorilerin dahil edilip edilmeyeceği</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Alt kategoriler listesi</returns>
    Task<IList<Category>> GetChildCategoriesAsync(
        int parentId, 
        bool includeInactive = false, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kategori hiyerarşisini (tree) getirir.
    /// </summary>
    /// <param name="includeInactive">Pasif kategorilerin dahil edilip edilmeyeceği</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Hiyerarşik kategori yapısı</returns>
    Task<IList<Category>> GetCategoryTreeAsync(
        bool includeInactive = false, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirtilen kategorinin üst kategoriler zincirini getirir.
    /// </summary>
    /// <param name="categoryId">Kategori ID'si</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Üst kategoriler listesi (en üstten başlayarak)</returns>
    Task<IList<Category>> GetParentCategoriesChainAsync(
        int categoryId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kategori adına göre arama yapar (case-insensitive).
    /// </summary>
    /// <param name="name">Aranacak kategori adı</param>
    /// <param name="exactMatch">Tam eşleşme aranacak mı</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Bulunan kategori (varsa)</returns>
    Task<Category?> GetByNameAsync(
        string name, 
        bool exactMatch = true, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ana kategorileri (parent'ı olmayan) getirir.
    /// </summary>
    /// <param name="includeInactive">Pasif kategorilerin dahil edilip edilmeyeceği</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Ana kategoriler listesi</returns>
    Task<IList<Category>> GetRootCategoriesAsync(
        bool includeInactive = false, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirtilen kategorinin ürünleri ile birlikte getirir.
    /// </summary>
    /// <param name="categoryId">Kategori ID'si</param>
    /// <param name="includeInactiveProducts">Pasif ürünlerin dahil edilip edilmeyeceği</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Ürünleri ile birlikte kategori</returns>
    Task<Category?> GetCategoryWithProductsAsync(
        int categoryId, 
        bool includeInactiveProducts = false, 
        CancellationToken cancellationToken = default);
}
