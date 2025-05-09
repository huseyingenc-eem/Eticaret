using ETicaret.Application.Services.Repositories;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

/// <summary>
/// Unit of Work deseninin somut implementasyonu.
/// DbContext'i ve repository örneklerini yönetir,
/// değişikliklerin tek bir transaction'da kaydedilmesini sağlar.
/// Hem IDisposable hem de IAsyncDisposable implemente eder.
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable // <<<=== IDisposable Eklendi
{
    private readonly BaseDBContexts _context;
    private bool disposed = false; // Dispose durumunu takip etmek için

    #region Private Repository Alanları
    // Repository alanları (lazy loading için private).
    private IAddressRepository? _addressRepository;
    private ICategoryRepository? _categoryRepository;
    private IOperationClaimRepository? _operationClaimRepository;
    private IOrderRepository? _orderRepository;
    private IOrderItemRepository? _orderItemRepository;
    private IProductRepository? _productRepository;
    private ISupplierRepository? _supplierRepository;
    //private IUserRepository? _userRepository;
    //private IRefreshTokenRepository? _refreshTokenRepository;
    private IProductImageRepository? _productImageRepository;
    private IProductVariantRepository? _productVariantRepository;
    private IReviewRepository? _reviewRepository;
    private IShoppingCartRepository? _shoppingCartRepository;
    private ICardItemRepository? _cardItemRepository;
    private IWishlistRepository? _wishlistRepository;
    private IWishlistItemRepository? _wishlistItemRepository;
    private IPaymentRepository? _paymentRepository;
    private IShipmentRepository? _shipmentRepository;
    private IShipmentItemRepository? _shipmentItemRepository;
    private IDiscountRepository? _discountRepository;
    private IDiscountUsageRepository? _discountUsageRepository;
    #endregion

    /// <summary>
    /// UnitOfWork sınıfının bir örneğini başlatır.
    /// </summary>
    /// <param name="context">Kullanılacak veritabanı context'i.</param>
    /// <exception cref="ArgumentNullException">Context null ise fırlatılır.</exception>
    public UnitOfWork(BaseDBContexts context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }



    #region Repository Property'leri
    // Repository property'leri (lazy loading ile ilk erişimde oluşturulur).

    #region Temel Varlık Repository'leri
    /// <summary>
    /// Adres işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IAddressRepository AddressRepository => _addressRepository ??= new AddressRepository(_context);
    /// <summary>
    /// Kategori işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(_context);
    /// <summary>
    /// Ürün işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IProductRepository ProductRepository => _productRepository ??= new ProductRepository(_context);
    /// <summary>
    /// Tedarikçi işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public ISupplierRepository SupplierRepository => _supplierRepository ??= new SupplierRepository(_context);
    ///// <summary>
    ///// Kullanıcı işlemleri için repository'ye erişim sağlar.
    ///// </summary>
    //public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
    #endregion

    #region Sipariş ve Ödeme Repository'leri
    /// <summary>
    /// Sipariş işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_context);
    /// <summary>
    /// Sipariş kalemleri için repository'ye erişim sağlar.
    /// </summary>
    public IOrderItemRepository OrderItemRepository => _orderItemRepository ??= new OrderItemRepository(_context);
    /// <summary>
    /// Ödeme işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);
    #endregion

    #region Kullanıcı Etkileşim Repository'leri
    /// <summary>
    /// Ürün yorumları (değerlendirmeler) için repository'ye erişim sağlar.
    /// </summary>
    public IReviewRepository ReviewRepository => _reviewRepository ??= new ReviewRepository(_context);
    /// <summary>
    /// Alışveriş sepeti işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IShoppingCartRepository ShoppingCartRepository => _shoppingCartRepository ??= new ShoppingCartRepository(_context);
    /// <summary>
    /// Alışveriş sepeti kalemleri için repository'ye erişim sağlar.
    /// </summary>
    public ICardItemRepository CardItemRepository => _cardItemRepository ??= new CardItemRepository(_context);
    /// <summary>
    /// İstek listesi işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IWishlistRepository WishlistRepository => _wishlistRepository ??= new WishlistRepository(_context);
    /// <summary>
    /// İstek listesi kalemleri için repository'ye erişim sağlar.
    /// </summary>
    public IWishlistItemRepository WishlistItemRepository => _wishlistItemRepository ??= new WishlistItemRepository(_context);
    #endregion

    #region Ürün Detay Repository'leri
    /// <summary>
    /// Ürün resimleri için repository'ye erişim sağlar.
    /// </summary>
    public IProductImageRepository ProductImageRepository => _productImageRepository ??= new ProductImageRepository(_context);
    /// <summary>
    /// Ürün varyantları için repository'ye erişim sağlar.
    /// </summary>
    public IProductVariantRepository ProductVariantRepository => _productVariantRepository ??= new ProductVariantRepository(_context);
    #endregion

    #region Kargo ve İndirim Repository'leri
    /// <summary>
    /// Kargo/Gönderi işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IShipmentRepository ShipmentRepository => _shipmentRepository ??= new ShipmentRepository(_context);
    /// <summary>
    /// Kargo/Gönderi kalemleri için repository'ye erişim sağlar.
    /// </summary>
    public IShipmentItemRepository ShipmentItemRepository => _shipmentItemRepository ??= new ShipmentItemRepository(_context);
    /// <summary>
    /// İndirim işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IDiscountRepository DiscountRepository => _discountRepository ??= new DiscountRepository(_context);
    /// <summary>
    /// İndirim kullanımı takibi için repository'ye erişim sağlar.
    /// </summary>
    public IDiscountUsageRepository DiscountUsageRepository => _discountUsageRepository ??= new DiscountUsageRepository(_context);
    #endregion
    #region Yetkilendirme Repository'leri
    /// <summary>
    /// Operasyon yetkileri (roller/izinler) için repository'ye erişim sağlar.
    /// </summary>
    public IOperationClaimRepository OperationClaimRepository => _operationClaimRepository ??= new OperationClaimRepository(_context);
    #endregion
    #endregion



    /// <summary>
    /// Değişiklikleri asenkron olarak kaydeder.
    /// </summary>
    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // --- IDisposable Implementasyonu ---

    /// <summary>
    /// UnitOfWork tarafından kullanılan kaynakları senkron olarak serbest bırakır.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Kaynakları serbest bırakan asıl metot (hem senkron hem asenkron için).
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
                _context.Dispose();
            disposed = true;
        }
    }

    // --- IAsyncDisposable Implementasyonu ---

    /// <summary>
    /// UnitOfWork tarafından kullanılan kaynakları asenkron olarak serbest bırakır.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        Dispose(disposing: false); 
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asenkron olarak kaynakları serbest bırakan asıl metot.
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (!this.disposed)
        {
            await _context.DisposeAsync();
        }
        // Yönetilmeyen kaynaklar (unmanaged resources) burada serbest bırakılır (varsa).
        // Eğer asenkron temizlik gerektiriyorsa burada yapılır.

    }
    ~UnitOfWork()
    {
        Dispose(disposing: false);
    }
}
