using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ETicaret.Persistence.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).IsRequired().HasMaxLength(150);
        builder.Property(d => d.DiscountCode).HasMaxLength(50);
        builder.HasIndex(d => d.DiscountCode).IsUnique().HasFilter("[DiscountCode] IS NOT NULL");

        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.DiscountType).IsRequired()
            .HasConversion(new EnumToStringConverter<DiscountType>());
        builder.Property(d => d.DiscountValue).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(d => d.MinimumPurchaseAmount).HasColumnType("decimal(18,2)");
        builder.Property(d => d.StartDate).IsRequired();
        builder.Property(d => d.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(d => d.IsGlobal).IsRequired().HasDefaultValue(false);

        // Discount to DiscountUsage (One-to-Many)
        builder.HasMany(d => d.Usages)
               .WithOne(du => du.Discount)
               .HasForeignKey(du => du.DiscountId)
               .OnDelete(DeleteBehavior.Cascade);

        // CreatedDate, UpdatedDate, DeletedDate (Base Entity'den)
        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}