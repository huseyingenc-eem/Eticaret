namespace ETicaret.Application.Features.UserRoles.Queries.UserWithRoles;

public class UserWithRolesResponseDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string City { get; set; }
    public IList<string> Roles { get; set; }
}
