namespace ETicaret.Application.Features.UserRoles.Commands.Update;
public class UpdateUserRolesResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<RoleOperationResult> AddedRoles { get; set; } = new();
    public List<RoleOperationResult> RemovedRoles { get; set; } = new();
    public List<string> CurrentRoles { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
}

public class RoleOperationResult
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; } = new();
}