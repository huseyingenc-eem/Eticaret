using Core.Application.Abstractions;
using ETicaret.Application.Services.Repositories; // Tüm repository arayüzleri için
using System; // IAsyncDisposable için
using System.Threading; // CancellationToken için
using System.Threading.Tasks; // Task için

namespace ETicaret.Application.Services.Repositories;

/// <summary>
/// Unit of Work desenini temsil eden arayüz.
/// Veritabanı işlemlerinin bir bütün olarak yönetilmesini ve atomik olmasını sağlar.
/// Repository'lere erişim noktası görevi görür ve değişikliklerin toplu olarak kaydedilmesini yönetir.
/// </summary>
public interface IUnitOfWork : Core.Application.Abstractions.IUnitOfWork
{
    #region Temel Varlık Repository'leri
    /// <summary>
    /// Adres işlemleri için repository'ye erişim sağlar.
    /// </summary>
    IAddressRepository AddressRepository { get; }

    /// <summary>
    /// Kategori işlemleri için repository'ye erişim sağlar.
    /// </summary>
    ICategoryRepository CategoryRepository { get; }

    /// <summary>
    /// Ürün işlemleri için repository'ye erişim sağlar.
    /// </summary>
    IProductRepository ProductRepository { get; }

    /// <summary>
    /// Tedarikçi işlemleri için repository'ye erişim sağlar.
    /// </summary>
    ISupplierRepository SupplierRepository { get; }

    ///// <summary>
    ///// Kullanıcı işlemleri için repository'ye erişim sağlar.
    ///// </summary>
    //IUserRepository UserRepository { get; }
    #endregion

    #region Sipariş ve Ödeme Repository'leri
    /// <summary>
    /// Sipariş işlemleri için repository'ye erişim sağlar.
    /// </summary>
    IOrderRepository OrderRepository { get; }

    /// <summary>
    /// Sipariş kalemleri için repository'ye erişim sağlar.
    /// </summary>
    IOrderItemRepository OrderItemRepository { get; }

    /// <summary>
    /// Ödeme işlemleri için repository'ye erişim sağlar.
    /// </summary>
    IPaymentRepository PaymentRepository { get; }
    #endregion

    #region Kullanıcı Etkileşim Repository'leri
    /// <summary>
    /// Ürün yorumları (değerlendirmeler) için repository'ye erişim sağlar.
    /// </summary>
    IReviewRepository ReviewRepository { get; }

    /// <summary>
    /// Alışveriş sepeti işlemleri için repository'ye erişim sağlar.
    /// </summary>
    IShoppingCartRepository ShoppingCartRepository { get; }

    /// <summary>
    /// Alışveriş sepeti kalemleri için repository'ye erişim sağlar.
    /// </summary>
    ICardItemRepository CardItemRepository { get; }

    /// <summary>
    /// İstek listesi işlemleri için repository'ye erişim sağlar.
    /// </summary>
    IWishlistRepository WishlistRepository { get; }

    /// <summary>
    /// İstek listesi kalemleri için repository'ye erişim sağlar.
    /// </summary>
    IWishlistItemRepository WishlistItemRepository { get; }
    #endregion

    #region Ürün Detay Repository'leri
    /// <summary>
    /// Ürün resimleri için repository'ye erişim sağlar.
    /// </summary>
    IProductImageRepository ProductImageRepository { get; }

    /// <summary>
    /// Ürün varyantları için repository'ye erişim sağlar.
    /// </summary>
    IProductVariantRepository ProductVariantRepository { get; }
    #endregion

    #region Kargo ve İndirim Repository'leri
    /// <summary>
    /// Kargo/Gönderi işlemleri için repository'ye erişim sağlar.
    /// </summary>
    IShipmentRepository ShipmentRepository { get; }

    /// <summary>
    /// Kargo/Gönderi kalemleri için repository'ye erişim sağlar.
    /// </summary>
    IShipmentItemRepository ShipmentItemRepository { get; }

    /// <summary>
    /// İndirim işlemleri için repository'ye erişim sağlar.
    /// </summary>
    IDiscountRepository DiscountRepository { get; }

    /// <summary>
    /// İndirim kullanımı takibi için repository'ye erişim sağlar.
    /// </summary>
    IDiscountUsageRepository DiscountUsageRepository { get; }
    #endregion

    #region Yetkilendirme Repository'leri
    /// <summary>
    /// Operasyon yetkileri (roller/izinler) için repository'ye erişim sağlar.
    /// </summary>
    IOperationClaimRepository OperationClaimRepository { get; }
    #endregion

}
