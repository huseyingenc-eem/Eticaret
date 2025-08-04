using Core.Application.Abstractions.Paging;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Application.Features.Addresses.Queries.GetList;
using ETicaret.Application.Features.Addresses.Queries.GetMyAddresses;
using ETicaret.Application.Features.Addresses.Queries.GetByUserId;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

[Authorize]
public class AddressController : BaseApiController
{
    public AddressController(IMediator mediator) : base(mediator)
    {
    }

    #region User Endpoints - Kullanıcının Kendi Adresleri

    [HttpGet("my-addresses")]
    [ProducesResponseType(typeof(List<GetMyAddressesResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyAddresses()
    {
        var query = new GetMyAddressesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("getbyid/{id}")]
    [ProducesResponseType(typeof(GetByIdAddressResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var query = new GetByIdAddressQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }


    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateAddressResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Add([FromBody] CreateAddressCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("update")]
    [ProducesResponseType(typeof(UpdateAddressResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateAddressCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(typeof(DeleteAddressResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteAddressCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Admin Endpoints - Yönetici İşlemleri

    /// <summary>
    /// Her kullanıcı için sadece default shipping adresini listeler.
    /// Admin panelinde kullanıcı başına tek adres gösterilir.
    /// </summary>
    [HttpGet("admin/default-shipping")]
    [ProducesResponseType(typeof(IPaginate<GetListAddressResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDefaultShippingAddresses([FromQuery] GetListAddressQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirli bir kullanıcının tüm adreslerini getirir (Admin yetkisi gerekli).
    /// Kullanıcı detay sayfası veya pop-up için kullanılır.
    /// </summary>
    [HttpGet("admin/user/{userId}/all")]
    [ProducesResponseType(typeof(IPaginate<GetByUserIdAddressResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserAllAddresses(
        [FromRoute] string userId)
    {

        var query = new GetByUserIdAddressQuery {UserId = userId};
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion
}