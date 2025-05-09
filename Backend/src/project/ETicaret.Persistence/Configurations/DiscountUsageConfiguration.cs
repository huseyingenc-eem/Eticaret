using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class DiscountUsageConfiguration : IEntityTypeConfiguration<DiscountUsage>
{
    public void Configure(EntityTypeBuilder<DiscountUsage> builder)
    {
        builder.ToTable("DiscountUsages");
        builder.HasKey(du => du.Id);

        builder.Property(du => du.DiscountId).IsRequired();
        builder.Property(du => du.UserId).IsRequired();
        builder.Property(du => du.OrderId).IsRequired();
        builder.Property(du => du.UsageDate).IsRequired();

        // DiscountUsage to Discount (Many-to-One)
        builder.HasOne(du => du.Discount)
               .WithMany(d => d.Usages)
               .HasForeignKey(du => du.DiscountId)
               .OnDelete(DeleteBehavior.Restrict);

        // DiscountUsage to User (Many-to-One)
        builder.HasOne(du => du.User)
               .WithMany() // User'da DiscountUsage'a direkt navigation property yoksa
               .HasForeignKey(du => du.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        // DiscountUsage to Order (Many-to-One)
        builder.HasOne(du => du.Order)
               .WithMany(o => o.AppliedDiscounts)
               .HasForeignKey(du => du.OrderId)
               .OnDelete(DeleteBehavior.Restrict);

        // CreatedDate alanı Entity<Guid>'dan geliyorsa:
        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}