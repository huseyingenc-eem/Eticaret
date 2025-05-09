using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("ShoppingCarts");

        builder.Property(sc => sc.UserId).IsRequired();
        builder.HasIndex(sc => sc.UserId).IsUnique();

        builder.HasOne(sc => sc.User)
               .WithOne(u => u.ShoppingCart)
               .HasForeignKey<ShoppingCart>(sc => sc.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // ShoppingCart to CartItem (One-to-Many)
        builder.HasMany(sc => sc.Items)
               .WithOne(ci => ci.ShoppingCart)
               .HasForeignKey(ci => ci.ShoppingCartId)
               .OnDelete(DeleteBehavior.Cascade);

        // CreatedDate, UpdatedDate, DeletedDate (Base Entity'den)
        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}