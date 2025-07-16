using ETicaret.Application.Features.Roles.Commands.Create;
using ETicaret.Application.Features.Roles.Commands.Delete;
using ETicaret.Application.Features.Roles.Commands.Update;
using ETicaret.Application.Features.Roles.Queries.RolesList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IMediator mediator) : ControllerBase
{

    [HttpPost("add")]
    public async Task<IActionResult> Add(CreateRoleCommand command)
    {
        var result = await mediator.Send(command);

        return Ok(result);
    }
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(string id)
    {
        var command = new DeleteRoleCommand { Id = id };
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(string id, UpdateRoleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route ID ve Command ID uyuşmuyor.");
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }
    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new RolesListQuery());

        return Ok(result);
    }
}
