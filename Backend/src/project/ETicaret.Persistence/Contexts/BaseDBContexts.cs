using ETicaret.Domain.Entities;
using ETicaret.Persistence.FirstDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ETicaret.Persistence.Contexts;

public class BaseDBContexts :IdentityDbContext<User,IdentityRole,string>
{
    public BaseDBContexts(DbContextOptions options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer();
    }


    public DbSet<Product> Products{ get; set; }
    public DbSet<Category> Categories{ get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }


    // Ürün Kataloğu ile İlgili DbSet'ler
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Review> Reviews { get; set; }

    // Kullanıcı Etkileşimi ile İlgili DbSet'ler
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<CardItem> CartItems { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }

    // Sipariş ve Teslimat ile İlgili Ek DbSet'ler
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentItem> ShipmentItems { get; set; }

    // Promosyonlar ile İlgili DbSet'ler
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<DiscountUsage> DiscountUsages { get; set; }

    // Çoktan Çoğa İlişki Ara Tabloları 
    // EF Core, bu tablolar için DbSet olmasa bile ilişkileri yönetebilir,
    public DbSet<DiscountProduct> DiscountProducts { get; set; }
    public DbSet<DiscountCategory> DiscountCategories { get; set; }
    public DbSet<asd> asds { get; set; }
}
