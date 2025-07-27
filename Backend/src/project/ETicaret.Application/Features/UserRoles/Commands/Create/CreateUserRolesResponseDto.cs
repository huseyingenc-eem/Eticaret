namespace ETicaret.Application.Features.UserRoles.Commands.Create;

public class CreateUserRolesResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
}