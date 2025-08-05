using ETicaret.Application.Features.Suppliers.Commands.Create;
using ETicaret.Application.Features.Suppliers.Commands.Update;
using ETicaret.Application.Features.Suppliers.Commands.Delete;
using ETicaret.Application.Features.Suppliers.Queries.GetById;
using ETicaret.Application.Features.Suppliers.Queries.GetList;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

public class SuppliersController : BaseApiController
{
    public SuppliersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] GetListSupplierQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        GetByIdSupplierQuery query = new() { Id = id };
        GetByIdSupplierResponseDto result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command)
    {
        CreateSupplierResponseDto result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateSupplierCommand command)
    {
        UpdateSupplierResponseDto result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        DeleteSupplierCommand command = new() { Id = id };
        DeleteSupplierResponseDto result = await _mediator.Send(command);
        return Ok(result);
    }
}