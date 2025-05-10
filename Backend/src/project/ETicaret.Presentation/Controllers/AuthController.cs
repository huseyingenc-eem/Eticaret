using ETicaret.Application.Features.Authentication.Command.Login;
using ETicaret.Application.Features.Authentication.Command.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ETicaret.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("current")]
    public IActionResult GetCurrentUser()
    {
        var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var roles = HttpContext.User.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList();

        return Ok(new
        {
            Id = UserId,
            Roles = roles
        });
    }
}
