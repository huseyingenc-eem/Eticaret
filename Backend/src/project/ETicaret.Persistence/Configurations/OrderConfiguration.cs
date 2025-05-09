using ETicaret.Domain.Entities;
using ETicaret.Domain.Enums; // OrderStatus için
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId).IsRequired();
            builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.Property(o => o.OrderDate).IsRequired();
            builder.Property(o => o.Status).IsRequired()
                .HasConversion(
                    v => v.ToString(), // Enum to string
                    v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v)); // string to Enum

            builder.Property(o => o.ShippingAddressId).IsRequired();
            builder.Property(o => o.BillingAddressId).IsRequired();

            builder.Property(o => o.Subtotal).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.DiscountTotal).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.ShippingFee).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.TaxTotal).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.GrandTotal).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.CustomerNotes).HasMaxLength(1000);

            // Order to User (Many-to-One)
            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order to ShippingAddress (Many-to-One)
            builder.HasOne(o => o.ShippingAddress)
                   .WithMany() // Address'in Order'a navigation property'si yoksa (veya birden fazla ise)
                   .HasForeignKey(o => o.ShippingAddressId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order to BillingAddress (Many-to-One)
            builder.HasOne(o => o.BillingAddress)
                   .WithMany()
                   .HasForeignKey(o => o.BillingAddressId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order to OrderItem (One-to-Many)
            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Order to Payment (One-to-Many)
            builder.HasMany(o => o.Payments)
                   .WithOne(p => p.Order)
                   .HasForeignKey(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Order to Shipment (One-to-Many)
            builder.HasMany(o => o.Shipments)
                   .WithOne(s => s.Order)
                   .HasForeignKey(s => s.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Order to DiscountUsage (One-to-Many)
            builder.HasMany(o => o.AppliedDiscounts)
                   .WithOne(du => du.Order)
                   .HasForeignKey(du => du.OrderId)
                   .OnDelete(DeleteBehavior.Restrict); 


            // CreatedDate, UpdatedDate, DeletedDate (Base Entity'den)
            builder.Property(e => e.CreatedTime).IsRequired();
            builder.Property(e => e.UpdateTime).IsRequired(false);
            builder.Property(e => e.DeletedTime).IsRequired(false);

        }
    }
}