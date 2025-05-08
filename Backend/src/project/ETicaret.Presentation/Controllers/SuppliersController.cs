using ETicaret.Application.Features.Suppliers.Commands.Create; // Add Command ve DTO
using ETicaret.Application.Features.Suppliers.Commands.Update; // Update Command ve DTO
using ETicaret.Application.Features.Suppliers.Commands.Delete; // Delete Command ve DTO
using ETicaret.Application.Features.Suppliers.Queries.GetById; // GetById Query ve DTO
using ETicaret.Application.Features.Suppliers.Queries.GetList; // GetList Query ve DTO
// using Core.Application.Requests; // Sayfalama için PageRequest
// using Core.Application.Responses; // Sayfalama için GetListResponse
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetList(/*[FromQuery] PageRequest pageRequest*/) // Sayfalama eklenirse
    {
        // Şimdilik sayfalama olmadan basit liste:
        GetListSupplierQuery getListSupplierQuery = new();
        var result = await _mediator.Send(getListSupplierQuery);
        return Ok(result);

        /* // Sayfalama ile:
        GetListSupplierQuery getListSupplierQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListSupplierResponseDto> result = await _mediator.Send(getListSupplierQuery);
        return Ok(result);
        */
    }

    [HttpGet("{id}")]
    // [Authorize]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        GetByIdSupplierQuery getByIdSupplierQuery = new() { Id = id };
        GetByIdSupplierResponseDto result = await _mediator.Send(getByIdSupplierQuery);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateSupplierCommand supplierAddCommand)
    {
        CreateSupplierResponseDto result = await _mediator.Send(supplierAddCommand);
        // Oluşturulan kaynağın URI'si ile 201 Created döndürmek daha RESTful olabilir:
        // return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSupplierCommand supplierUpdateCommand)
    {
        UpdateSupplierResponseDto result = await _mediator.Send(supplierUpdateCommand);
        return Ok(result);
        // Alternatif olarak sadece başarı durumu için NoContent(204) döndürülebilir.
        // return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        SupplierDeleteCommand supplierDeleteCommand = new() { Id = id };
        SupplierDeleteResponseDto result = await _mediator.Send(supplierDeleteCommand);
        return Ok(result);
        // Alternatif olarak sadece başarı durumu için NoContent(204) döndürülebilir.
        // return NoContent();
    }
}