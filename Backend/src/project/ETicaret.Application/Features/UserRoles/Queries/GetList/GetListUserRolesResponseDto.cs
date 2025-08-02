namespace ETicaret.Application.Features.UserRoles.Queries.GetList;

public class GetListUserRolesResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string RoleNames { get; set; } = string.Empty;
}