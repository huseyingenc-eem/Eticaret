using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems").HasKey(oi => oi.Id); // Tablo adı ve Primary Key

        // Property Konfigürasyonları
        builder.Property(oi => oi.Id).HasColumnName("Id").IsRequired();
        builder.Property(oi => oi.OrderId).HasColumnName("OrderId").IsRequired();
        builder.Property(oi => oi.ProductId).HasColumnName("ProductId").IsRequired();
        builder.Property(oi => oi.Quantity).HasColumnName("Quantity").IsRequired();
        builder.Property(oi => oi.Price).HasColumnName("Price").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(oi => oi.CreatedTime).HasColumnName("CreatedTime").IsRequired();
        builder.Property(oi => oi.UpdateTime).HasColumnName("UpdateTime");

        // İlişkiler (Relationships)

        // OrderItem -> Order (Bir Sipariş Kaleminin bir Siparişi olur)
        // Bu ilişki zaten OrderConfiguration tarafında tanımlandı (HasMany ile).
        // Burada tekrar tanımlamaya gerek yok ama istenirse WithOne kısmı burada da belirtilebilir.
        // builder.HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId);

        // OrderItem -> Product (Bir Sipariş Kaleminin bir Ürünü olur)
        builder.HasOne(oi => oi.Product)
               .WithMany() // Bir ürün birden çok sipariş kaleminde bulunabilir
               .HasForeignKey(oi => oi.ProductId)
               .OnDelete(DeleteBehavior.Restrict); // Ürün silinirse, geçmiş siparişlerdeki ilişkili kalemler hata verir (silinmez)
    }
}