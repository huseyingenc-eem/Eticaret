using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Abstraction;


[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    public readonly IMediator _mediator;

    protected BaseApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
