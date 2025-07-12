using ETicaret.Application.Services.Repositories;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

/// <summary>
/// Unit of Work deseninin somut implementasyonu.
/// DbContext'i ve repository örneklerini yönetir,
/// değişikliklerin tek bir transaction'da kaydedilmesini sağlar.
/// Hem IDisposable hem de IAsyncDisposable implemente eder.
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly BaseDBContexts _context;
    private bool disposed = false;

    #region Repository Dependencies
    private readonly IAddressRepository _addressRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ICardItemRepository _cardItemRepository;
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IWishlistItemRepository _wishlistItemRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IShipmentItemRepository _shipmentItemRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IDiscountUsageRepository _discountUsageRepository;
    #endregion

    /// <summary>
    /// UnitOfWork sınıfının bir örneğini başlatır.
    /// </summary>
    /// <param name="context">Kullanılacak veritabanı context'i.</param>
    /// <param name="addressRepository">Adres repository'si.</param>
    /// <param name="categoryRepository">Kategori repository'si.</param>
    /// <param name="operationClaimRepository">Operasyon yetki repository'si.</param>
    /// <param name="orderRepository">Sipariş repository'si.</param>
    /// <param name="orderItemRepository">Sipariş kalem repository'si.</param>
    /// <param name="productRepository">Ürün repository'si.</param>
    /// <param name="supplierRepository">Tedarikçi repository'si.</param>
    /// <param name="productImageRepository">Ürün resim repository'si.</param>
    /// <param name="productVariantRepository">Ürün varyant repository'si.</param>
    /// <param name="reviewRepository">Değerlendirme repository'si.</param>
    /// <param name="shoppingCartRepository">Sepet repository'si.</param>
    /// <param name="cardItemRepository">Sepet kalem repository'si.</param>
    /// <param name="wishlistRepository">İstek listesi repository'si.</param>
    /// <param name="wishlistItemRepository">İstek listesi kalem repository'si.</param>
    /// <param name="paymentRepository">Ödeme repository'si.</param>
    /// <param name="shipmentRepository">Kargo repository'si.</param>
    /// <param name="shipmentItemRepository">Kargo kalem repository'si.</param>
    /// <param name="discountRepository">İndirim repository'si.</param>
    /// <param name="discountUsageRepository">İndirim kullanım repository'si.</param>
    /// <exception cref="ArgumentNullException">Herhangi bir parametre null ise fırlatılır.</exception>
    public UnitOfWork(
        BaseDBContexts context,
        IAddressRepository addressRepository,
        ICategoryRepository categoryRepository,
        IOperationClaimRepository operationClaimRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IProductImageRepository productImageRepository,
        IProductVariantRepository productVariantRepository,
        IReviewRepository reviewRepository,
        IShoppingCartRepository shoppingCartRepository,
        ICardItemRepository cardItemRepository,
        IWishlistRepository wishlistRepository,
        IWishlistItemRepository wishlistItemRepository,
        IPaymentRepository paymentRepository,
        IShipmentRepository shipmentRepository,
        IShipmentItemRepository shipmentItemRepository,
        IDiscountRepository discountRepository,
        IDiscountUsageRepository discountUsageRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _operationClaimRepository = operationClaimRepository ?? throw new ArgumentNullException(nameof(operationClaimRepository));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _orderItemRepository = orderItemRepository ?? throw new ArgumentNullException(nameof(orderItemRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
        _productImageRepository = productImageRepository ?? throw new ArgumentNullException(nameof(productImageRepository));
        _productVariantRepository = productVariantRepository ?? throw new ArgumentNullException(nameof(productVariantRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
        _cardItemRepository = cardItemRepository ?? throw new ArgumentNullException(nameof(cardItemRepository));
        _wishlistRepository = wishlistRepository ?? throw new ArgumentNullException(nameof(wishlistRepository));
        _wishlistItemRepository = wishlistItemRepository ?? throw new ArgumentNullException(nameof(wishlistItemRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _shipmentRepository = shipmentRepository ?? throw new ArgumentNullException(nameof(shipmentRepository));
        _shipmentItemRepository = shipmentItemRepository ?? throw new ArgumentNullException(nameof(shipmentItemRepository));
        _discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
        _discountUsageRepository = discountUsageRepository ?? throw new ArgumentNullException(nameof(discountUsageRepository));
    }

    #region Repository Properties
    #region Temel Varlık Repository'leri
    /// <summary>
    /// Adres işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IAddressRepository AddressRepository => _addressRepository;

    /// <summary>
    /// Kategori işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public ICategoryRepository CategoryRepository => _categoryRepository;

    /// <summary>
    /// Ürün işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IProductRepository ProductRepository => _productRepository;

    /// <summary>
    /// Tedarikçi işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public ISupplierRepository SupplierRepository => _supplierRepository;
    #endregion

    #region Sipariş ve Ödeme Repository'leri
    /// <summary>
    /// Sipariş işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IOrderRepository OrderRepository => _orderRepository;

    /// <summary>
    /// Sipariş kalemleri için repository'ye erişim sağlar.
    /// </summary>
    public IOrderItemRepository OrderItemRepository => _orderItemRepository;

    /// <summary>
    /// Ödeme işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IPaymentRepository PaymentRepository => _paymentRepository;
    #endregion

    #region Kullanıcı Etkileşim Repository'leri
    /// <summary>
    /// Ürün yorumları (değerlendirmeler) için repository'ye erişim sağlar.
    /// </summary>
    public IReviewRepository ReviewRepository => _reviewRepository;

    /// <summary>
    /// Alışveriş sepeti işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IShoppingCartRepository ShoppingCartRepository => _shoppingCartRepository;

    /// <summary>
    /// Alışveriş sepeti kalemleri için repository'ye erişim sağlar.
    /// </summary>
    public ICardItemRepository CardItemRepository => _cardItemRepository;

    /// <summary>
    /// İstek listesi işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IWishlistRepository WishlistRepository => _wishlistRepository;

    /// <summary>
    /// İstek listesi kalemleri için repository'ye erişim sağlar.
    /// </summary>
    public IWishlistItemRepository WishlistItemRepository => _wishlistItemRepository;
    #endregion

    #region Ürün Detay Repository'leri
    /// <summary>
    /// Ürün resimleri için repository'ye erişim sağlar.
    /// </summary>
    public IProductImageRepository ProductImageRepository => _productImageRepository;

    /// <summary>
    /// Ürün varyantları için repository'ye erişim sağlar.
    /// </summary>
    public IProductVariantRepository ProductVariantRepository => _productVariantRepository;
    #endregion

    #region Kargo ve İndirim Repository'leri
    /// <summary>
    /// Kargo/Gönderi işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IShipmentRepository ShipmentRepository => _shipmentRepository;

    /// <summary>
    /// Kargo/Gönderi kalemleri için repository'ye erişim sağlar.
    /// </summary>
    public IShipmentItemRepository ShipmentItemRepository => _shipmentItemRepository;

    /// <summary>
    /// İndirim işlemleri için repository'ye erişim sağlar.
    /// </summary>
    public IDiscountRepository DiscountRepository => _discountRepository;

    /// <summary>
    /// İndirim kullanımı takibi için repository'ye erişim sağlar.
    /// </summary>
    public IDiscountUsageRepository DiscountUsageRepository => _discountUsageRepository;
    #endregion

    #region Yetkilendirme Repository'leri
    /// <summary>
    /// Operasyon yetkileri (roller/izinler) için repository'ye erişim sağlar.
    /// </summary>
    public IOperationClaimRepository OperationClaimRepository => _operationClaimRepository;
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
    }

    ~UnitOfWork()
    {
        Dispose(disposing: false);
    }
}
