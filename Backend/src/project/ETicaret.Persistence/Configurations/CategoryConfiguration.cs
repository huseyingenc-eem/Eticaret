using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Tablo adı (isteğe bağlı)
        // builder.ToTable("Kategoriler");

        // Alan Konfigürasyonları
        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Description)
               .IsRequired(false);

        builder.Property(c => c.IsActive)
               .HasDefaultValue(true);

        // İlişkiler

        // 1. Kendi Kendine İlişki (Hiyerarşi: Parent/Children)
        builder.HasOne(c => c.Parent)
               .WithMany(c => c.Children)
               .HasForeignKey(c => c.ParentId)
               .IsRequired(false)
               // Dikkat: Cascade delete, üst kategori silindiğinde TÜM alt kategorileri veritabanından SİLER.
               // Bu işlem geri alınamaz. Uyarı mekanizması frontend/backend'de KURULMALIDIR.
               .OnDelete(DeleteBehavior.Cascade); // Davranış Cascade olarak değiştirildi.

        // 2. Product İlişkisi (Category -> Products)
        builder.HasMany(c => c.Products) // Bir kategorinin birden çok ürünü olabilir
               .WithOne(p => p.Category) // Bir ürünün bir tane kategorisi olur
               .HasForeignKey(p => p.CategoryID) // Product entity'sindeki foreign key alanı CategoryID'dir
               .IsRequired() // Her ürünün bir kategorisi olmak zorunda (Product.CategoryID non-nullable olduğu için)
                             // Kategori silinirse ürünlerin silinmesini engelle:
               .OnDelete(DeleteBehavior.Restrict); // Davranış Restrict olarak ayarlandı.

    }
}