using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

public class WishlistItem : Entity<Guid>
{
    public Guid WishlistId { get; set; }
    public Guid ProductVariantId { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Wishlist Wishlist { get; set; }
    public virtual ProductVariant ProductVariant { get; set; }
}