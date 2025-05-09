using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class CardItem : Entity<Guid>
{
    public Guid ShoppingCartId { get; set; }
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtAddition { get; set; } // Sepete eklendiği andaki fiyat

    // Navigation Properties
    public virtual ShoppingCart ShoppingCart { get; set; }
    public virtual ProductVariant ProductVariant { get; set; }
}