using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class ProductImage : Entity<Guid>
{
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }
    public bool IsMain { get; set; } = false;
    public string ImageUrl { get; set; }
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; } = 0;

    
}
