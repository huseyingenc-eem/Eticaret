using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Tablo adı (isteğe bağlı)
        // builder.ToTable("Kategoriler");

        // Alan Konfigürasyonları
        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Description)
               .IsRequired(false);

        builder.Property(c => c.IsActive)
               .HasDefaultValue(true);

        // İlişkiler

        // 1. Kendi Kendine İlişki (Hiyerarşi: Parent/Children)
        builder.HasOne(c => c.Parent)
               .WithMany(c => c.Children)
               .HasForeignKey(c => c.ParentId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        // 2. Product İlişkisi (Category -> Products)
        builder.HasMany(c => c.Products)
               .WithOne(p => p.Category)
               .HasForeignKey(p => p.CategoryID)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

    }
}