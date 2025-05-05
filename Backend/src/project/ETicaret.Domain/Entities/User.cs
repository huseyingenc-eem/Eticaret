using Microsoft.AspNetCore.Identity;

namespace ETicaret.Domain.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? City { get; set; }

    public virtual ICollection<Address> Addresses { get; set; }
    //public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
