using Core.Application.Abstractions;
using ETicaret.Application.Services.Repositories;
using ETicaret.Persistence.Contexts;
using ETicaret.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaret.Persistence;

/// <summary>
/// Persistence katmanı servislerini IServiceCollection'a eklemek için genişletme metotları içerir.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Persistence katmanı için gerekli servisleri (DbContext, UnitOfWork, Repository'ler)
    /// Dependency Injection container'ına ekler.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <param name="configuration">Uygulama yapılandırması.</param>
    /// <returns>Yapılandırılmış servis koleksiyonu.</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Veritabanı context'ini SQL Server kullanarak kaydeder.
        // Bağlantı dizesi appsettings.json dosyasından alınır.
        services.AddDbContext<BaseDBContexts>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
            opt.EnableSensitiveDataLogging();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>(); // ETicaret.Application...IUnitOfWork -> UnitOfWork
        services.AddScoped<ICoreUnitOfWork, UnitOfWork>(); // Core.Application...ICoreUnitOfWork -> UnitOfWork

        #region Repository Kayıtları
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<ICardItemRepository, CardItemRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IWishlistItemRepository, WishlistItemRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<IShipmentItemRepository, ShipmentItemRepository>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddScoped<IDiscountUsageRepository, DiscountUsageRepository>();
        #endregion

        return services;
    }
}
