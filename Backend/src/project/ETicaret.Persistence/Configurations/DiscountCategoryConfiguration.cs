using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class DiscountCategoryConfiguration : IEntityTypeConfiguration<DiscountCategory>
{
    public void Configure(EntityTypeBuilder<DiscountCategory> builder)
    {
        builder.ToTable("DiscountCategories");
        // Composite Primary Key
        builder.HasKey(dc => new { dc.DiscountId, dc.CategoryId });

        // DiscountCategory to Discount (Many-to-One)
        builder.HasOne(dc => dc.Discount)
               .WithMany(d => d.ApplicableCategories)
               .HasForeignKey(dc => dc.DiscountId)
               .OnDelete(DeleteBehavior.Cascade);

        // DiscountCategory to Category (Many-to-One)
        builder.HasOne(dc => dc.Category)
               .WithMany(c => c.DiscountCategories)
               .HasForeignKey(dc => dc.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}