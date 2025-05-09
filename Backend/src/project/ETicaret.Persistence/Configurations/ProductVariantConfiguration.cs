using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.ProductId).IsRequired();
        builder.Property(pv => pv.Sku).IsRequired().HasMaxLength(100);
        builder.HasIndex(pv => pv.Sku).IsUnique(); // SKU benzersiz olmalı

        builder.Property(pv => pv.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(pv => pv.CompareAtPrice).HasColumnType("decimal(18,2)");
        builder.Property(pv => pv.UnitsInStock).IsRequired().HasDefaultValue(0);
        builder.Property(pv => pv.VariantImageUrl).HasMaxLength(255);
        builder.Property(pv => pv.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(pv => pv.AttributeDescription).HasMaxLength(500); // Renk:Kırmızı,Beden:XL gibi

        // ProductVariant to Product (Many-to-One)
        builder.HasOne(pv => pv.Product)
               .WithMany(p => p.Variants)
               .HasForeignKey(pv => pv.ProductId)
               .OnDelete(DeleteBehavior.Cascade); // Ana ürün silinince bu da silinsin

        // ProductVariant to OrderItem (One-to-Many)
        builder.HasMany(pv => pv.OrderItems)
               .WithOne(oi => oi.ProductVariant)
               .HasForeignKey(oi => oi.ProductVariantId)
               .OnDelete(DeleteBehavior.Restrict); // Bu varyant bir siparişte varsa silinemez

        // ProductVariant to CartItem (One-to-Many)
        builder.HasMany(pv => pv.CartItems)
               .WithOne(ci => ci.ProductVariant)
               .HasForeignKey(ci => ci.ProductVariantId)
               .OnDelete(DeleteBehavior.Cascade); // Varyant silinirse sepetlerden de kalksın

        // ProductVariant to WishlistItem (One-to-Many)
        builder.HasMany(pv => pv.WishlistItems)
              .WithOne(wi => wi.ProductVariant)
              .HasForeignKey(wi => wi.ProductVariantId)
              .OnDelete(DeleteBehavior.Cascade); // Varyant silinirse istek listelerinden de kalksın

        // CreatedDate, UpdatedDate, DeletedDate (Base Entity'den)
        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}