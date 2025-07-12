using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

public class ShoppingCart : Entity<Guid>
{
    public string UserId { get; set; }

    public virtual User User { get; set; }
    public virtual ICollection<CardItem> Items { get; set; } = new HashSet<CardItem>();
}
