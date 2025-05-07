using ETicaret.Application.Services.Repositories; // IUnitOfWork ve diğer repository arayüzleri için
using ETicaret.Persistence.Contexts; // BaseDBContexts için
using System;
using System.Threading;
using System.Threading.Tasks;

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

    // Repository örnekleri (lazy-load)
    private OrderRepository? _orderRepository;
    private OrderItemRepository? _orderItemRepository;
    private ProductRepository? _productRepository;
    private AddressRepository? _addressRepository;
    private SupplierRepository? _supplierRepository;
    private CategoryRepository? _categoryRepository;
    private OperationClaimRepository? _operationClaimRepository; // Seeder için bu da lazım olabilir

    public UnitOfWork(BaseDBContexts context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Repository Property'leri
    public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_context);
    public IOrderItemRepository OrderItemRepository => _orderItemRepository ??= new OrderItemRepository(_context);
    public IProductRepository ProductRepository => _productRepository ??= new ProductRepository(_context);
    public IAddressRepository AddressRepository => _addressRepository ??= new AddressRepository(_context);
    public ISupplierRepository SupplierRepository => _supplierRepository ??= new SupplierRepository(_context);
    public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(_context);
    // OperationClaimRepository'yi de ekleyelim (eğer IUnitOfWork arayüzünde varsa veya gerekiyorsa)
    // Eğer IUnitOfWork'de yoksa, bu property'yi eklemeyin veya arayüzü güncelleyin.
    // public IOperationClaimRepository OperationClaimRepository => _operationClaimRepository ??= new OperationClaimRepository(_context);


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
        GC.SuppressFinalize(this); // Finalizer'ı baskıla
    }

    /// <summary>
    /// Kaynakları serbest bırakan asıl metot (hem senkron hem asenkron için).
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // Yönetilen kaynakları (managed resources) dispose et.
                // DbContext'in senkron Dispose'unu çağır.
                _context.Dispose();
            }

            // Yönetilmeyen kaynaklar (unmanaged resources) burada serbest bırakılır (varsa).
            // Örneğin: file handles, native connections vb.

            disposed = true;
        }
    }

    // --- IAsyncDisposable Implementasyonu ---

    /// <summary>
    /// UnitOfWork tarafından kullanılan kaynakları asenkron olarak serbest bırakır.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        // Asenkron dispose işlemini yap.
        await DisposeAsyncCore();

        // Senkron dispose metodu ile aynı temel temizliği yap (opsiyonel ama iyi pratik).
        Dispose(disposing: false); // Yönetilen kaynaklar zaten DisposeAsyncCore'da halledildi.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asenkron olarak kaynakları serbest bırakan asıl metot.
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (!this.disposed) // Henüz dispose edilmediyse
        {
            // Yönetilen kaynakları asenkron olarak dispose et.
            // DbContext'in asenkron DisposeAsync'ını çağır.
            await _context.DisposeAsync();
        }

        // Yönetilmeyen kaynaklar (unmanaged resources) burada serbest bırakılır (varsa).
        // Eğer asenkron temizlik gerektiriyorsa burada yapılır.

        // Dispose durumunu burada tekrar set etmeye gerek yok, DisposeAsync zaten Dispose(false)'u çağırıyor.
        // disposed = true; // Bu satıra gerek yok
    }

    // İsteğe bağlı: Finalizer (Eğer yönetilmeyen kaynaklar varsa)
    // ~UnitOfWork()
    // {
    //     Dispose(disposing: false);
    // }
}
