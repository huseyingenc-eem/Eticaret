using ETicaret.Domain.Entities;
using ETicaret.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ETicaret.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.Property(p => p.OrderId).IsRequired();
        builder.Property(p => p.UserId).IsRequired(false);
        builder.Property(p => p.PaymentMethod).IsRequired().HasMaxLength(50);
        builder.Property(p => p.PaymentProviderTransactionId).HasMaxLength(100);
        builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.PaymentStatus)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<PaymentStatus>());
        builder.Property(p => p.PaymentDate).IsRequired();

        // Payment to Order (Many-to-One)
        builder.HasOne(p => p.Order)
               .WithMany(o => o.Payments)
               .HasForeignKey(p => p.OrderId)
               .OnDelete(DeleteBehavior.Cascade); // Sipariş silinirse ödemesi de silinsin

        // Payment to User (Many-to-One, nullable)
        builder.HasOne(p => p.User)
               .WithMany(u => u.PaymentsMade)
               .HasForeignKey(p => p.UserId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull); // Kullanıcı silinirse ödemedeki UserId null olsun

        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}