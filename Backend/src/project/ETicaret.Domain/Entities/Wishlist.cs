using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

public class Wishlist : Entity<Guid>
{
    public string UserId { get; set; }
    
    public virtual User User { get; set; }
    public virtual ICollection<WishlistItem> Items { get; set; } = new HashSet<WishlistItem>();
}
