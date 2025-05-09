using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.OrderId).IsRequired();
        builder.Property(s => s.TrackingNumber).HasMaxLength(100);
        builder.Property(s => s.ShippingCarrier).HasMaxLength(100);
        builder.Property(s => s.ShipmentStatus).IsRequired().HasMaxLength(50);

        // Shipment to Order (Many-to-One)
        builder.HasOne(s => s.Order)
               .WithMany(o => o.Shipments)
               .HasForeignKey(s => s.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        // Shipment to ShipmentItem (One-to-Many)
        builder.HasMany(s => s.ShipmentItems)
               .WithOne(si => si.Shipment)
               .HasForeignKey(si => si.ShipmentId)
               .OnDelete(DeleteBehavior.Cascade);

        // CreatedDate, UpdatedDate, DeletedDate (Base Entity'den)
        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}