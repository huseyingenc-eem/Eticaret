using ETicaret.Application.Features.Categories.Commands.Create;
using ETicaret.Application.Features.Categories.Commands.Delete;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Application.Features.Categories.Queries.GetCategoryTree;
using ETicaret.Application.Features.Categories.Queries.GetCategoryWithProducts;
using ETicaret.Application.Features.Categories.Queries.GetChildCategories;
using ETicaret.Application.Features.Categories.Queries.GetParentCategories;
using MediatR;
using ETicaret.Presentation.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

public class CategoryController : BaseApiController
{
    public CategoryController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return Created("", result);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        DeleteCategoryCommand command = new() { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Tüm kategorileri ağaç yapısı (parent-child) şeklinde getirir.
    /// </summary>
    /// <returns>Kategori ağacı DTO listesi</returns>
    [HttpGet("GetCategoryTree")]
    public async Task<IActionResult> GetCategoryTree()
    {
        var query = new GetCategoryTreeQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen kategoriye ait ürünleri birlikte getirir.
    /// </summary>
    /// <param name="id">Kategori ID'si</param>
    /// <returns>Kategori bilgisi ve ürün listesi</returns>
    [HttpGet("GetCategoryWithProducts/{id}")]
    public async Task<IActionResult> GetCategoryWithProducts(int id)
    {
        var query = new GetCategoryWithProductsQuery
        {
            Id = id
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }




    /// <summary>
    /// Belirli bir üst kategoriye ait alt kategorileri getirir.
    /// </summary>
    /// <param name="parentId">Üst kategori ID'si</param>
    /// <returns>Alt kategorilerin listesi</returns>
    [HttpGet("GetChildCategories/{parentId}")]
    public async Task<IActionResult> GetChildCategories([FromRoute] int parentId)
    {
        var query = new GetChildCategoriesQuery
        {
            ParentId = parentId
        };

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Tüm üst (ana) kategorileri getirir. Yani ParentId'si null olan kategoriler.
    /// </summary>
    /// <returns>Üst kategorilerin listesi</returns>
    [HttpGet("GetParentCategories")]
    public async Task<IActionResult> GetParentCategories()
    {
        var query = new GetParentCategoriesQuery();

        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
