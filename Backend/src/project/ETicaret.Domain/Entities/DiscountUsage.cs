using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

public class DiscountUsage : Entity<Guid>
{
    public Guid DiscountId { get; set; }
    public string UserId { get; set; }
    public Guid OrderId { get; set; }
    public DateTime UsageDate { get; set; } = DateTime.UtcNow;
    public virtual Discount Discount { get; set; }
    public virtual User User { get; set; }
    public virtual Order Order { get; set; }
}
