namespace ETicaret.Application.Features.UserRoles.Commands.Delete;

public class DeleteUserRoleResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string RemovedRoleId { get; set; } = string.Empty;
    public string RemovedRoleName { get; set; } = string.Empty;
    public List<string> RemainingRoles { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
}