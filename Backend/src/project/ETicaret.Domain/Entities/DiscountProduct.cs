namespace ETicaret.Domain.Entities;

public class DiscountProduct
{
    public Guid DiscountId { get; set; }
    public Guid ProductId { get; set; }

    public virtual Discount Discount { get; set; }
    public virtual Product Product { get; set; }
}
