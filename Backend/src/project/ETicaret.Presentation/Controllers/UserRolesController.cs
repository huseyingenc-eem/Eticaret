using ETicaret.Application.Features.UserRoles.Commands.Create;
using ETicaret.Application.Features.UserRoles.Queries.UserWithRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController(IMediator mediator) : ControllerBase
    {

        [HttpPost("add")]
        public async Task<IActionResult> Add(UserRolesAddCommand command)
        {
            var result = await mediator.Send(command);

            return Ok(result);
        }


        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserWithRoles(string id)
        {
            var response = new UserWithRolesQuery { UserId = id };
            var result = await mediator.Send(response);

            return Ok(result);
        }
    }
}
