using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.OrderId).IsRequired();
        builder.Property(oi => oi.ProductVariantId).IsRequired();
        builder.Property(oi => oi.Quantity).IsRequired();
        builder.Property(oi => oi.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");

        // OrderItem to Order (Many-to-One)
        builder.HasOne(oi => oi.Order)
               .WithMany(o => o.OrderItems)
               .HasForeignKey(oi => oi.OrderId)
               .OnDelete(DeleteBehavior.Cascade); // OrderConfiguration'da zaten tanımlı

        // OrderItem to ProductVariant (Many-to-One)
        builder.HasOne(oi => oi.ProductVariant)
               .WithMany(pv => pv.OrderItems)
               .HasForeignKey(oi => oi.ProductVariantId)
               .OnDelete(DeleteBehavior.Restrict); // Sipariş kalemi olan bir varyant direkt silinememeli

        // OrderItem to ShipmentItem (One-to-Many)
        builder.HasMany(oi => oi.ShipmentItems)
               .WithOne(si => si.OrderItem)
               .HasForeignKey(si => si.OrderItemId)
               .OnDelete(DeleteBehavior.Cascade); // Sipariş kalemi silinirse kargo kalemleri de silinsin

        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}