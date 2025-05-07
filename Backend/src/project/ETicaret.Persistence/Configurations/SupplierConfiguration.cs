using ETicaret.Domain.Entities; // Supplier ve Product entity'leri için
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {

        builder.Property(s => s.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(s => s.ContactPerson)
               .IsRequired(false)
               .HasMaxLength(100);

        builder.Property(s => s.ContactEmail)
               .IsRequired(false)
               .HasMaxLength(100);

        builder.Property(s => s.PhoneNumber)
               .IsRequired(false)
               .HasMaxLength(20);

        builder.Property(s => s.Address)
               .IsRequired(false);

        builder.Property(s => s.IsActive)
               .HasDefaultValue(true);

        builder.HasMany(s => s.Products) 
               .WithOne(p => p.Supplier)
               .HasForeignKey(p => p.SupplierID)
               .IsRequired() 
               .OnDelete(DeleteBehavior.Restrict);
    }
}