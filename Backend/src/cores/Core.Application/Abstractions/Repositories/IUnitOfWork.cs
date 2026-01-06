using Core.Domain.Entities;

namespace Core.Application.Abstractions.Repositories;

/// <summary>
/// Temel Unit of Work işlemlerini, repository erişimini ve transaction yönetimini tanımlayan arayüz.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    #region Repository Erişimi

    /// <summary>
    /// Belirtilen varlık tipi için genel (generic) repository'nin bir örneğini döndürür.
    /// Bu metot, servislerin ihtiyaç duyduğu repository'lere tek bir merkezden (Unit of Work)
    /// erişmesini sağlayarak constructor (yapıcı metot) kalabalığını azaltır.
    /// 
    /// Örnek Kullanım:
    /// <code>
    /// public class OrderProcessingService
    /// {
    ///     private readonly ICoreUnitOfWork _unitOfWork;
    ///
    ///     // Servis, sadece IUnitOfWork'e bağımlı.
    ///     public OrderProcessingService(ICoreUnitOfWork unitOfWork)
    ///     {
    ///         _unitOfWork = unitOfWork;
    ///     }
    ///
    ///     public async Task ProcessOrder(int productId, int customerId)
    ///     {
    ///         // İhtiyaç anında repository'ler UoW üzerinden alınır.
    ///         var productRepository = _unitOfWork.GetRepository&lt;Product, int&gt;();
    ///         var customerRepository = _unitOfWork.GetRepository&lt;Customer, int&gt;();
    ///
    ///         var product = await productRepository.GetByIdAsync(productId);
    ///         var customer = await customerRepository.GetByIdAsync(customerId);
    ///         // ... işlemler ...
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <typeparam name="TEntity">Repository'si istenen varlık tipi.</typeparam>
    /// <typeparam name="TId">Varlığın ID tipi.</typeparam>
    /// <returns><c>IRepository</c> arayüzünün bir örneği.</returns>
    //TRepository GetRepository<TRepository>() where TRepository : class;


    #endregion

    #region Transaction Yönetimi

    /// <summary>
    /// Yeni bir veritabanı işlemi (transaction) asenkron olarak başlatır.
    /// Bu metottan sonra yapılan tüm veritabanı işlemleri, <c>CommitTransactionAsync</c> veya
    /// <c>RollbackTransactionAsync</c> çağrılana kadar tek bir atomik işlem olarak kabul edilir.
    /// 
    /// Örnek Kullanım:
    /// <code>
    /// public async Task CreateOrderAsync(CreateOrderDto orderDto)
    /// {
    ///     await _unitOfWork.BeginTransactionAsync(); // 1. Transaction'ı başlat
    ///     try
    ///     {
    ///         var orderRepository = _unitOfWork.GetRepository&lt;Order, Guid&gt;();
    ///         var productRepository = _unitOfWork.GetRepository&lt;Product, int&gt;();
    ///
    ///         // Siparişi ekle
    ///         var order = new Order { ... };
    ///         await orderRepository.AddAsync(order);
    ///
    ///         // Ürün stoğunu düşür
    ///         var product = await productRepository.GetByIdAsync(orderDto.ProductId);
    ///         product.Stock -= orderDto.Quantity;
    ///
    ///         await _unitOfWork.CompleteAsync();      // 2. Değişiklikleri kaydet
    ///         await _unitOfWork.CommitTransactionAsync(); // 3. Her şey yolundaysa onayla
    ///     }
    ///     catch (Exception)
    ///     {
    ///         await _unitOfWork.RollbackTransactionAsync(); // 4. Hata olursa her şeyi geri al
    ///         throw;
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>İşlemi temsil eden bir görev.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <c>BeginTransactionAsync</c> ile başlatılan mevcut işlemi onaylar ve yapılan değişiklikleri
    /// veritabanında kalıcı hale getirir. Genellikle bir <c>try</c> bloğunun sonunda çağrılır.
    /// </summary>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>İşlemi temsil eden bir görev.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <c>BeginTransactionAsync</c> ile başlatılan mevcut işlem sırasında bir hata oluşursa,
    /// o ana kadar yapılmış tüm değişiklikleri geri alır. Genellikle bir <c>catch</c> bloğunun
    /// içinde çağrılarak veri tutarlılığını güvence altına alır.
    /// </summary>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>İşlemi temsil eden bir görev.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Kaydetme İşlemi

    /// <summary>
    /// Bu Unit of Work kapsamında yapılan tüm değişiklikleri (Add, Update, Delete) veritabanına asenkron olarak kaydeder.
    /// Eğer bir transaction başlatıldıysa, bu metot o transaction'ın bir parçası olarak çalışır.
    /// Eğer transaction yoksa, kendi başına bir işlem olarak tüm değişiklikleri kaydeder.
    /// 
    /// Örnek Kullanım:
    /// <code>
    /// // Transaction gerektirmeyen basit bir işlem
    /// public async Task UpdateProfileName(int userId, string newName)
    /// {
    ///     var userRepository = _unitOfWork.GetRepository&lt;User, int&gt;();
    ///     var user = await userRepository.GetByIdAsync(userId);
    ///     user.Name = newName;
    ///     
    ///     await _unitOfWork.CompleteAsync(); // Değişikliği doğrudan kaydet
    /// }
    /// </code>
    /// </summary>
    /// <param name="cancellationToken">İşlemin iptal edilip edilemeyeceğini belirten bir token.</param>
    /// <returns>Veritabanında etkilenen satır sayısını içeren bir görev.</returns>
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);

    #endregion
}