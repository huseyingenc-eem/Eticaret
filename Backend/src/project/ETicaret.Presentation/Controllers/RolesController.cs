using ETicaret.Application.Features.Roles.Commands.Create;
using ETicaret.Application.Features.Roles.Queries.RolesList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IMediator mediator) : ControllerBase
{

    [HttpPost("add")]
    public async Task<IActionResult> Add(RolesAddCommand command)
    {
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
