using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class Review : Entity<Guid>
{
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }

    public string? UserId { get; set; }
    public virtual User? User { get; set; }

    public int Rating { get; set; } 
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = false;

    
}