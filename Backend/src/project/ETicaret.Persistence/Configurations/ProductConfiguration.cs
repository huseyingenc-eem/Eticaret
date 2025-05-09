using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Navigation(x => x.Category).AutoInclude();
        builder.Navigation(x => x.Supplier).AutoInclude();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);

        builder.Property(p => p.CategoryId).IsRequired();
        builder.Property(p => p.SupplierId).IsRequired(false);

        builder.Property(p => p.Description)
               .IsRequired(false);


        // Product to Category (Many-to-One)
        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        // Product to Supplier (Many-to-One, nullable)
        builder.HasOne(p => p.Supplier)
               .WithMany(s => s.Products)
               .HasForeignKey(p => p.SupplierId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        // Product to ProductVariant (One-to-Many)
        builder.HasMany(p => p.Variants)
               .WithOne(pv => pv.Product)
               .HasForeignKey(pv => pv.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        // Product to ProductImage (One-to-Many)
        builder.HasMany(p => p.Images)
               .WithOne(pi => pi.Product)
               .HasForeignKey(pi => pi.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        // Product to Review (One-to-Many)
        builder.HasMany(p => p.Reviews)
               .WithOne(r => r.Product)
               .HasForeignKey(r => r.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}