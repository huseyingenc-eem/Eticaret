using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("Wishlists");
        builder.HasKey(wl => wl.Id);

        builder.Property(wl => wl.UserId).IsRequired();
        builder.HasIndex(wl => wl.UserId).IsUnique();


        // Wishlist to User (One-to-One or One-to-Many if Name exists)
        builder.HasOne(wl => wl.User)
               .WithOne(u => u.Wishlist) // Eğer tek wishlist ise WithOne, çoklu ise WithMany
               .HasForeignKey<Wishlist>(wl => wl.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // Wishlist to WishlistItem (One-to-Many)
        builder.HasMany(wl => wl.Items)
               .WithOne(wi => wi.Wishlist)
               .HasForeignKey(wi => wi.WishlistId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}