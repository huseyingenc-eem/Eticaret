using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");

        builder.Property(pi => pi.ProductId).IsRequired();
        builder.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(255);
        builder.Property(pi => pi.AltText).HasMaxLength(200);
        builder.Property(pi => pi.DisplayOrder).IsRequired().HasDefaultValue(0);
        builder.Property(pi => pi.IsMain).IsRequired().HasDefaultValue(false);
        

        // ProductImage to Product (Many-to-One)
        builder.HasOne(pi => pi.Product)
               .WithMany(p => p.Images)
               .HasForeignKey(pi => pi.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}