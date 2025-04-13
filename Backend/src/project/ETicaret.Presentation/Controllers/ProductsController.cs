using ETicaret.Application.Features.Products.Commands.Create;
using ETicaret.Application.Features.Products.Commands.Delete;
using ETicaret.Application.Features.Products.Commands.Update;
using ETicaret.Application.Features.Products.Queries.GetAllByCategoryId;
using ETicaret.Application.Features.Products.Queries.GetById;
using ETicaret.Application.Features.Products.Queries.GetDetails;
using ETicaret.Application.Features.Products.Queries.GetList;
using ETicaret.Application.Features.Products.Queries.GetListNameContains;
using ETicaret.Application.Features.Products.Queries.GetListPriceRange;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Add(ProductAddCommand command)
        {
            string result = await mediator.Send(command);
            return Ok(result);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ProductDeleteCommand command = new ProductDeleteCommand { Id = id };
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(ProductUpdateCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            GetlistProductQuery query = new GetlistProductQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetAllDetails()
        {
            var result = await mediator.Send(new GetDetailsProductQuery());
            return Ok(result);
        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await mediator.Send(new GetByIdProductQuery() 
            { 
                Id=id
            });
            return Ok(result);
        }

        

        [HttpGet("getallbycategory")]
        public async Task<IActionResult> GetAllByCategoryId(int categoryId)
        {
            var query = new GetAllByCategoryIdProductQuery { CategoryId = categoryId };
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("getallpricerange")]
        public async Task<IActionResult> GetAllByPriceRange(double min ,double max)
        {
            var query = new GetListProductPriceRangeQuery() 
            { 
                Min=min,
                Max=max
            };
            var result = await mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("getallbynamecontains")]
        public async Task<IActionResult> GetAllByNameContains(string text)
        {
            var query = new GetListProductNameContainsQuery()
            {
                Text=text
            };
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
