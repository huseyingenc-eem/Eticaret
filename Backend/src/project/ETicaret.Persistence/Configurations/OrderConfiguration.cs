using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        

        builder.ToTable("Orders").HasKey(o => o.Id);

        builder.Navigation(o => o.OrderItems).AutoInclude();
        builder.Navigation(o => o.ShippingAddress).AutoInclude();
        builder.Navigation(o => o.BillingAddress).AutoInclude();


        builder.Property(o => o.Id).HasColumnName("Id").IsRequired();
        builder.Property(o => o.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(o => o.OrderDate).HasColumnName("OrderDate").IsRequired();
        builder.Property(o => o.TotalAmount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(o => o.Status).HasColumnName("Status").IsRequired()
               .HasConversion<string>();
        builder.Property(o => o.ShippingAddressId).HasColumnName("ShippingAddressId").IsRequired();
        builder.Property(o => o.BillingAddressId).HasColumnName("BillingAddressId").IsRequired();
        builder.Property(o => o.CreatedTime).HasColumnName("CreatedTime").IsRequired();
        builder.Property(o => o.UpdateTime).HasColumnName("UpdateTime");

        
        builder.HasOne(o => o.User);

       
        builder.HasOne(o => o.ShippingAddress)
               .WithMany()
               .HasForeignKey(o => o.ShippingAddressId)
               .OnDelete(DeleteBehavior.Restrict); 

      
        builder.HasOne(o => o.BillingAddress)
               .WithMany()
               .HasForeignKey(o => o.BillingAddressId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
               .WithOne(oi => oi.Order)
               .HasForeignKey(oi => oi.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}