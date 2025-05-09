using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.Property(ci => ci.ShoppingCartId).IsRequired();
            builder.Property(ci => ci.ProductVariantId).IsRequired();
            builder.Property(ci => ci.Quantity).IsRequired();
            builder.Property(ci => ci.PriceAtAddition).IsRequired().HasColumnType("decimal(18,2)");


            // CartItem to ShoppingCart (Many-to-One)
            builder.HasOne(ci => ci.ShoppingCart)
                   .WithMany(sc => sc.Items)
                   .HasForeignKey(ci => ci.ShoppingCartId)
                   .OnDelete(DeleteBehavior.Cascade);

            // CartItem to ProductVariant (Many-to-One)
            builder.HasOne(ci => ci.ProductVariant)
                   .WithMany(pv => pv.CartItems)
                   .HasForeignKey(ci => ci.ProductVariantId)
                   .OnDelete(DeleteBehavior.Cascade); 

            // CreatedDate, UpdatedDate, DeletedDate (Base Entity'den)
            builder.Property(e => e.CreatedTime).IsRequired();
            builder.Property(e => e.UpdateTime).IsRequired(false);
            builder.Property(e => e.DeletedTime).IsRequired(false);

        }
    }
}