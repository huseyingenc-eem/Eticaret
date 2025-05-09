using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class ShipmentItemConfiguration : IEntityTypeConfiguration<ShipmentItem>
{
    public void Configure(EntityTypeBuilder<ShipmentItem> builder)
    {
        builder.ToTable("ShipmentItems");
        builder.HasKey(si => si.Id);

        builder.Property(si => si.ShipmentId).IsRequired();
        builder.Property(si => si.OrderItemId).IsRequired();
        builder.Property(si => si.QuantityShipped).IsRequired();

        // ShipmentItem to Shipment (Many-to-One)
        builder.HasOne(si => si.Shipment)
               .WithMany(s => s.ShipmentItems)
               .HasForeignKey(si => si.ShipmentId)
               .OnDelete(DeleteBehavior.Cascade); 

        // ShipmentItem to OrderItem (Many-to-One)
        builder.HasOne(si => si.OrderItem)
               .WithMany(oi => oi.ShipmentItems)
               .HasForeignKey(si => si.OrderItemId)
               .OnDelete(DeleteBehavior.Restrict); 

        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}