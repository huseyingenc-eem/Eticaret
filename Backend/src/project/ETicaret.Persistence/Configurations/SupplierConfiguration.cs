using ETicaret.Domain.Entities; // Supplier ve Product entity'leri için
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {

        // Name Alanı (Şirket Adı)
        builder.Property(s => s.Name)
               .IsRequired() // Zorunlu alan
               .HasMaxLength(150); // Örnek maksimum uzunluk

        // Diğer İsteğe Bağlı Alanlar
        builder.Property(s => s.ContactPerson)
               .IsRequired(false) // İsteğe bağlı
               .HasMaxLength(100); // Örnek maksimum uzunluk

        builder.Property(s => s.ContactEmail)
               .IsRequired(false) // İsteğe bağlı
               .HasMaxLength(100); // Örnek maksimum uzunluk

        builder.Property(s => s.PhoneNumber)
               .IsRequired(false) // İsteğe bağlı
               .HasMaxLength(20); // Örnek maksimum uzunluk

        builder.Property(s => s.Address)
               .IsRequired(false); // İsteğe bağlı

        // IsActive Alanı
        builder.Property(s => s.IsActive)
               .HasDefaultValue(true); // Varsayılan değer true

        // İlişkiler

        // 1. Product İlişkisi (Supplier -> Products)
        builder.HasMany(s => s.Products) // Bir tedarikçinin birden çok ürünü olabilir
               .WithOne(p => p.Supplier) // Bir ürünün bir tane tedarikçisi olur
               .HasForeignKey(p => p.SupplierID) // Product entity'sindeki foreign key alanı SupplierID'dir
               .IsRequired() // Her ürünün bir tedarikçisi olmak zorunda (Product.SupplierID non-nullable olduğu için)
                             // Tedarikçi silinirse ürünlerin silinmesini engelle:
               .OnDelete(DeleteBehavior.Restrict); // Davranış Restrict olarak ayarlandı. İçinde ürün olan tedarikçi silinemez.
    }
}