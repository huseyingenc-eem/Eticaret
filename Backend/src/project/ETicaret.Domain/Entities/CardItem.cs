using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

public class CardItem : Entity<Guid>
{
    public Guid ShoppingCartId { get; set; }
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtAddition { get; set; }
    public virtual ShoppingCart ShoppingCart { get; set; }
    public virtual ProductVariant ProductVariant { get; set; }
}