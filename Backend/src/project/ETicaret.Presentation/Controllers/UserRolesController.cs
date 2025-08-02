using ETicaret.Application.Features.UserRoles.Commands.Create;
using ETicaret.Application.Features.UserRoles.Commands.Delete;
using ETicaret.Application.Features.UserRoles.Commands.Update;
using ETicaret.Application.Features.UserRoles.Queries.GetList;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

/// <summary>
/// Kullanıcı-rol yönetimi için API endpoint'lerini sağlayan controller.
/// </summary>
[Authorize]
public class UserRolesController : BaseApiController
{
    public UserRolesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Tüm kullanıcıları ve rolleri sayfalanmış şekilde getirir.
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetUserRolesList([FromQuery] GetListUserRolesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcıya rol atar.
    /// </summary>
    [HttpPost("assign")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] CreateUserRolesCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcının rollerini günceller (ekle/kaldır).
    /// </summary>
    [HttpPut("user/{userId}/roles")]
    public async Task<IActionResult> UpdateUserRoles(
        [FromRoute] string userId,
        [FromBody] UpdateUserRolesCommand command)
    {
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcıdan rol kaldırır.
    /// </summary>
    [HttpDelete("user/{userId}/role/{roleId}")]
    public async Task<IActionResult> RemoveRoleFromUser(
        [FromRoute] string userId,
        [FromRoute] string roleId)
    {
        var command = new DeleteUserRoleCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}