namespace ETicaret.Domain.Entities;

public class DiscountCategory
{
    public Guid DiscountId { get; set; }
    public int CategoryId { get; set; }

    public virtual Discount Discount { get; set; }
    public virtual Category Category { get; set; }
}