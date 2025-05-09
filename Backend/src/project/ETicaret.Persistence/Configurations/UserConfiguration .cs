using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);


            builder.HasMany(u => u.Addresses)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinince adresleri de silinsin 

            builder.HasMany(u => u.Orders)
                   .WithOne(o => o.User)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // Kullanıcının siparişi varken kullanıcı silinemez

            builder.HasMany(u => u.Reviews)
                   .WithOne(r => r.User)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinince yorumları da silinsin

            builder.HasOne(u => u.ShoppingCart)
                   .WithOne(sc => sc.User)
                   .HasForeignKey<ShoppingCart>(sc => sc.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Wishlist)
                   .WithOne(wl => wl.User)
                   .HasForeignKey<Wishlist>(wl => wl.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PaymentsMade)
                   .WithOne(p => p.User)
                   .HasForeignKey(p => p.UserId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull); // Kullanıcı silinirse ödemelerdeki UserId null olsun

            
        }
    }
}