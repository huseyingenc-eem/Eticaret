using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class DiscountProductConfiguration : IEntityTypeConfiguration<DiscountProduct>
{
    public void Configure(EntityTypeBuilder<DiscountProduct> builder)
    {
        builder.ToTable("DiscountProducts");
        builder.HasKey(dp => new { dp.DiscountId, dp.ProductId });

        // DiscountProduct to Discount (Many-to-One)
        builder.HasOne(dp => dp.Discount)
               .WithMany(d => d.ApplicableProducts)
               .HasForeignKey(dp => dp.DiscountId)
               .OnDelete(DeleteBehavior.Cascade); // İndirim silinirse bu ilişki de silinsin

        // DiscountProduct to Product (Many-to-One)
        builder.HasOne(dp => dp.Product)
               .WithMany(p => p.DiscountProducts)
               .HasForeignKey(dp => dp.ProductId)
               .OnDelete(DeleteBehavior.Cascade); // Ürün silinirse bu ilişki de silinsin
    }
}