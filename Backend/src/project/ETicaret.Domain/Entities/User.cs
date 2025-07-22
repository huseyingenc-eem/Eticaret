using Microsoft.AspNetCore.Identity;

namespace ETicaret.Domain.Entities;

public class User : IdentityUser<string>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? City { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    public virtual ShoppingCart? ShoppingCart { get; set; }
    public virtual Wishlist? Wishlist { get; set; }
    public virtual ICollection<Payment> PaymentsMade { get; set; } = new HashSet<Payment>();
}

