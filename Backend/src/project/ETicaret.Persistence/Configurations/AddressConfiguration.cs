using ETicaret.Domain.Entities; // Address ve User için
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

/// <summary>
/// Address entity'si için veritabanı yapılandırmasını tanımlar.
/// </summary>
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        // Tablo adı (isteğe bağlı, varsayılan: "Addresses")
        builder.ToTable("Addresses");

        // Alan Konfigürasyonları
        builder.Property(a => a.UserId)
               .IsRequired(); // Kullanıcı ID'si zorunlu

        builder.Property(a => a.AddressTitle)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(a => a.Country)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.City)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.District)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.Street)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(a => a.FullAddress)
               .IsRequired()
               .HasMaxLength(250);

        builder.Property(a => a.PostalCode)
               .IsRequired(false) // İsteğe bağlı
               .HasMaxLength(10);

        builder.Property(a => a.IsBillingAddress)
               .HasDefaultValue(false); // Varsayılan değer

        builder.Property(a => a.IsShippingAddress)
               .HasDefaultValue(false); // Varsayılan değer

        // İlişki: Address -> User (Bire-Çok)
        builder.HasOne(a => a.User) // Bir adresin bir kullanıcısı vardır
               .WithMany(u => u.Addresses) // Bir kullanıcının birden çok adresi olabilir (User entity'sinde Addresses koleksiyonu olmalı)
               .HasForeignKey(a => a.UserId) // Foreign key UserId'dir
               .IsRequired() // Her adres bir kullanıcıya bağlı olmalı
               .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse adresleri de silinsin (Yaygın senaryo)
                                                  // Veya DeleteBehavior.Restrict (Kullanıcı silinemez) veya SetNull (UserId null olur - pek mantıklı değil)
    }
}
