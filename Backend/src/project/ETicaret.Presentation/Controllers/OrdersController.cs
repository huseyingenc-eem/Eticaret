using ETicaret.Application.Features.Orders.Commands.Create;
using ETicaret.Application.Features.Orders.Commands.UpdateStatus;
using ETicaret.Application.Features.Orders.Queries.GetById;
using ETicaret.Application.Features.Orders.Queries.GetListByUserId;
using ETicaret.Application.Features.Orders.Queries.GetListForEmployee;
using ETicaret.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderAddCommand createOrderCommand)
    {
        OrderAddResponseDto response = await _mediator.Send(createOrderCommand);
        return CreatedAtAction(nameof(GetOrderById), new { orderId = response.OrderId }, response);
    }

    [HttpPut("{orderId:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus([FromRoute] int orderId, [FromBody] OrderUpdateStatusCommand.OrderUpdateStatusCommandHandler payload)
    {

        var command = new OrderUpdateStatusCommand
        {
            OrderId = orderId,
            NewStatus = payload.NewStatus
        };
        OrderUpdateStatusResponse response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderById([FromRoute] int orderId)
    {
        GetOrderByIdQuery query = new GetOrderByIdQuery(orderId);
        GetOrderByIdResponseDto response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        GetOrdersByUserIdQuery query = new GetOrdersByUserIdQuery();
        List<GetOrdersByUserIdResponseDto> response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("employee/list")]
    public async Task<IActionResult> GetOrderListForEmployee([FromQuery] OrderStatus? statusFilter)
    {
        GetOrderListForEmployeeQuery query = new GetOrderListForEmployeeQuery
        {
            StatusFilter = statusFilter
        };
        List<GetOrderListForEmployeeResponseDto> response = await _mediator.Send(query);
        return Ok(response);
    }
}
