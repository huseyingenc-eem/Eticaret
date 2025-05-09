using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.ToTable("WishlistItems");

        builder.Property(wi => wi.WishlistId).IsRequired();
        builder.Property(wi => wi.ProductVariantId).IsRequired();
        builder.Property(wi => wi.DateAdded).IsRequired();

        // WishlistItem to Wishlist (Many-to-One)
        builder.HasOne(wi => wi.Wishlist)
               .WithMany(wl => wl.Items)
               .HasForeignKey(wi => wi.WishlistId)
               .OnDelete(DeleteBehavior.Cascade);

        // WishlistItem to ProductVariant (Many-to-One)
        builder.HasOne(wi => wi.ProductVariant)
               .WithMany(pv => pv.WishlistItems)
               .HasForeignKey(wi => wi.ProductVariantId)
               .OnDelete(DeleteBehavior.Cascade);

        // CreatedDate, UpdatedDate, DeletedDate (Base Entity'den)
        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);
    }
}