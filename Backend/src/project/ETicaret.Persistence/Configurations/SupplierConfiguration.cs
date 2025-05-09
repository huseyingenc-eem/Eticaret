using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.Property(s => s.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.ContactPerson).HasMaxLength(100);
        builder.Property(s => s.ContactEmail).HasMaxLength(100);
        builder.Property(s => s.PhoneNumber).HasMaxLength(20);
        builder.Property(s => s.Address).HasMaxLength(500);

        // Supplier to Product (One-to-Many)
        builder.HasMany(s => s.Products)
               .WithOne(p => p.Supplier)
               .HasForeignKey(p => p.SupplierId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull); // Tedarikçi silinirse, ürünlerin SupplierId'si null olur.

        // CreatedDate, UpdatedDate, DeletedDate (Base Entity'den)
        builder.Property(e => e.CreatedTime).IsRequired();
        builder.Property(e => e.UpdateTime).IsRequired(false);
        builder.Property(e => e.DeletedTime).IsRequired(false);

    }
}