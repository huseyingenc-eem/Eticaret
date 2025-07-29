using ETicaret.Application.Features.Authentication.Command.Login;
using ETicaret.Application.Features.Authentication.Command.Register;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ETicaret.Presentation.Controllers;

public class AuthController: BaseApiController 
{

    public AuthController(IMediator mediator) : base(mediator) { }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("current")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Bu kontrol, [Authorize]'dan geçmiş ama token'ı hatalı olan bir durumu yakalar.
        if (string.IsNullOrEmpty(userId))
        {
            return new ObjectResult("Token geçerli fakat gerekli kullanıcı kimliği bilgisini içermiyor.")
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        var roles = HttpContext.User.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList();

        return Ok(new
        {
            Id = userId,
            Roles = roles
        });
    }
}
