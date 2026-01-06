using Core.Application.Common.Results;
using ETicaret.Application.Features.ProductVariants.Commands.Create;
using ETicaret.Application.Features.ProductVariants.Commands.Delete;
using ETicaret.Application.Features.ProductVariants.Commands.Update;
using ETicaret.Application.Features.ProductVariants.Queries.GetById;
using ETicaret.Application.Features.ProductVariants.Queries.GetByProduct;
using ETicaret.Application.Features.ProductVariants.Queries.GetList;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

public class ProductVariantsController : BaseApiController
{
    public ProductVariantsController(IMediator mediator) : base(mediator)
    {
    }

    #region Query Endpoints


    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetByIdProductVariantResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var query = new GetByIdProductVariantQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("product/{productId:guid}/variants")]
    [ProducesResponseType(typeof(List<GetByProductVariantsResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProduct([FromRoute] Guid productId)
    {
        var query = new GetByProductVariantsQuery
        {
            ProductId = productId,
            OnlyActive = true
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("admin/list")]
    [Authorize]
    [ProducesResponseType(typeof(PagedResult<GetListProductVariantResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetList([FromQuery] GetListProductVariantQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Command Endpoints (Admin Only)

    [HttpPost("create")]
    [Authorize]
    [ProducesResponseType(typeof(CreateProductVariantResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateProductVariantCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("update")]
    [Authorize]
    [ProducesResponseType(typeof(UpdateProductVariantResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromBody] UpdateProductVariantCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(DeleteProductVariantResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, [FromQuery] bool isHardDelete = false)
    {
        var command = new DeleteProductVariantCommand
        {
            Id = id,
            IsHardDelete = isHardDelete
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("admin/hard-delete/{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(DeleteProductVariantResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> HardDelete([FromRoute] Guid id)
    {
        var command = new DeleteProductVariantCommand
        {
            Id = id,
            IsHardDelete = true
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Utility Endpoints

    [HttpGet("product/{productId:guid}/total-stock")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductTotalStock([FromRoute] Guid productId)
    {
        var query = new GetByProductVariantsQuery
        {
            ProductId = productId,
            OnlyActive = true
        };
        var variants = await _mediator.Send(query);
        var totalStock = variants.Sum(v => v.UnitsInStock);

        return Ok(new { ProductId = productId, TotalStock = totalStock });
    }

    [HttpGet("admin/low-stock")]
    [Authorize]
    [ProducesResponseType(typeof(PagedResult<GetListProductVariantResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetLowStockVariants([FromQuery] int threshold = 10, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
    {
        var query = new GetListProductVariantQuery
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            OnlyLowStock = true,
            LowStockThreshold = threshold,
            IsActiveFilter = true
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("admin/check-sku/{sku}")]
    [Authorize]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckSkuAvailability([FromRoute] string sku, [FromQuery] Guid? excludeId = null)
    {
        var query = new GetListProductVariantQuery
        {
            SkuFilter = sku,
            PageSize = 1
        };
        var result = await _mediator.Send(query);

        bool isAvailable = !result.Items.Any(v => excludeId == null || v.Id != excludeId);

        return Ok(new { Sku = sku, IsAvailable = isAvailable });
    }

    #endregion
}