using Microsoft.AspNetCore.Identity;

namespace ETicaret.Domain.Entities;

public class User : IdentityUser
{
    public string? City { get; set; }

}
