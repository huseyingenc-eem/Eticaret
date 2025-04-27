using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Mevcut AutoInclude
        builder.Navigation(x => x.Category).AutoInclude();
        // Yeni Eklenen AutoInclude (Tedarikçi bilgisi sıkça gerekiyorsa)
        builder.Navigation(x => x.Supplier).AutoInclude();

        // Alan Konfigürasyonları
        builder.Property(p => p.Name)
               .IsRequired() // Zorunlu alan
               .HasMaxLength(200); // Maksimum uzunluk

        builder.Property(p => p.Price)
               .IsRequired() // Zorunlu alan
               .HasColumnType("decimal(18,2)"); // Veritabanı tipi ve hassasiyet

        builder.Property(p => p.Stock)
               .IsRequired(); // Zorunlu alan

        builder.Property(p => p.CategoryID)
               .IsRequired(); // Zorunlu alan

        builder.Property(p => p.SupplierID)
               .IsRequired(); // Zorunlu alan

        // Yeni eklenen alanlar
        builder.Property(p => p.Description)
               .IsRequired(false); // İsteğe bağlı (nullable)

        builder.Property(p => p.SKU)
               .IsRequired(false) // İsteğe bağlı (nullable)
               .HasMaxLength(100); // Maksimum uzunluk

        builder.Property(p => p.ImageUrl)
               .IsRequired(false); // İsteğe bağlı (nullable)

        builder.Property(p => p.IsActive)
               .HasDefaultValue(true); // Varsayılan değer

        // SKU için benzersiz index (isteğe bağlı)
        builder.HasIndex(p => p.SKU)
               .IsUnique();

        // İlişkiler (Zaten EF Core tarafından convention ile veya migration'da tanımlanmış olabilir,
        // ancak burada açıkça belirtmek iyi olabilir)
        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products) // Category entity'sinde Products koleksiyonu varsa
               .HasForeignKey(p => p.CategoryID)
               .OnDelete(DeleteBehavior.Restrict); // Kategori silinirse ürünler ne olacak? (Restrict: Silinemez)

        builder.HasOne(p => p.Supplier)
               .WithMany(s => s.Products) // Supplier entity'sinde Products koleksiyonu varsa
               .HasForeignKey(p => p.SupplierID)
               .OnDelete(DeleteBehavior.Restrict); // Tedarikçi silinirse ürünler ne olacak?
    }
}