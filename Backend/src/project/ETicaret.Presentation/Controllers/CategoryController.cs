using ETicaret.Application.Features.Categories.Commands.Create;
using ETicaret.Application.Features.Categories.Commands.Delete;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Application.Features.Categories.Commands.UpdateRange;
using ETicaret.Application.Features.Categories.Queries.GetById;
using ETicaret.Application.Features.Categories.Queries.GetCategoryTree;
using ETicaret.Application.Features.Categories.Queries.GetChildCategories;
using ETicaret.Application.Features.Categories.Queries.GetParentCategories;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

public class CategoryController : BaseApiController
{

    public CategoryController( IMediator mediator) : base(mediator) {}

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return Created("", result);
    }

    [HttpDelete("delete/{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {

        DeleteCategoryCommand command = new() { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update(UpdateCategoryCommand command)
    {

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("update-range")]
    [Authorize]
    public async Task<IActionResult> AddRange([FromBody] UpdateRangeCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return Created("", result);
    }

    [HttpGet("GetCategoryTree")]
    public async Task<IActionResult> GetCategoryTree()
    {
        var query = new GetCategoryTreeQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetChildCategories/{parentId}")]
    public async Task<IActionResult> GetChildCategories([FromRoute] int parentId)
    {
        var query = new GetChildCategoriesQuery { ParentId = parentId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetParentCategories")]
    public async Task<IActionResult> GetParentCategories()
    {
        var query = new GetParentCategoriesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetById/{id:int}")]
    [ProducesResponseType(typeof(GetByIdCategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var query = new GetByIdCategoryQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

}