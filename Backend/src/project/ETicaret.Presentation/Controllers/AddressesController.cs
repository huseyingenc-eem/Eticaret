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

/// <summary>
/// Adres yönetimi için API endpoint'lerini sağlayan controller.
/// </summary>
[Authorize]
public class AddressController : BaseApiController
{
    public AddressController(IMediator mediator) : base(mediator)
    {
    }

    #region User Endpoints - Kullanıcının Kendi Adresleri

    /// <summary>
    /// Giriş yapmış kullanıcının tüm adreslerini listeler.
    /// IRequestInfoRequest sayesinde UserId otomatik olarak atanır.
    /// </summary>
    /// <returns>Kullanıcının adreslerinin listesi.</returns>
    /// <response code="200">Kullanıcının adres listesi başarıyla döndürüldü.</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    [HttpGet("my-addresses")]
    [ProducesResponseType(typeof(List<GetMyAddressesResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyAddresses()
    {
        // IRequestInfoRequest sayesinde UserId middleware tarafından otomatik atanır
        var query = new GetMyAddressesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip adresi getirir (Sadece kullanıcının kendi adresi ise).
    /// </summary>
    /// <param name="id">Getirilecek adresin ID'si.</param>
    /// <returns>Adres detayları.</returns>
    /// <response code="200">Adres detayları başarıyla döndürüldü.</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    /// <response code="404">Belirtilen ID'ye sahip adres bulunamadı veya kullanıcıya ait değil.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetByIdAddressResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        // IRequestInfoRequest varsa UserId otomatik atanır
        var query = new GetByIdAddressQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Giriş yapmış kullanıcı için yeni bir adres ekler.
    /// </summary>
    /// <param name="command">Eklenecek adres bilgileri.</param>
    /// <returns>Eklenen adresin bilgileri.</returns>
    /// <response code="201">Adres başarıyla oluşturuldu.</response>
    /// <response code="400">Geçersiz istek verisi (Validation hatası).</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateAddressResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Add([FromBody] CreateAddressCommand command)
    {
        // IRequestInfoRequest varsa UserId otomatik atanır
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Kullanıcının adresini günceller.
    /// </summary>
    /// <param name="command">Güncellenecek adres bilgileri.</param>
    /// <returns>Güncellenmiş adres bilgileri.</returns>
    /// <response code="200">Adres başarıyla güncellendi.</response>
    /// <response code="400">Geçersiz istek verisi.</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    /// <response code="403">Kullanıcının bu adresi güncelleme yetkisi yok.</response>
    /// <response code="404">Güncellenecek adres bulunamadı.</response>
    [HttpPut("update")]
    [ProducesResponseType(typeof(UpdateAddressResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateAddressCommand command)
    {
        // IRequestInfoRequest varsa UserId otomatik atanır
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip adresi siler (Sadece kullanıcının kendi adresi ise).
    /// </summary>
    /// <param name="id">Silinecek adresin ID'si.</param>
    /// <returns>İşlem sonucu.</returns>
    /// <response code="200">Adres başarıyla silindi.</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    /// <response code="403">Kullanıcının bu adresi silme yetkisi yok.</response>
    /// <response code="404">Silinecek adres bulunamadı.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(DeleteAddressResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        // IRequestInfoRequest varsa UserId otomatik atanır
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
    /// <param name="query">Liste sorgusu parametreleri.</param>
    /// <returns>Default shipping adresleri listesi (sayfalanmış).</returns>
    /// <response code="200">Default shipping adresleri başarıyla döndürüldü.</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için Admin yetkisi gerekli.</response>
    [HttpGet("admin/default-shipping")]
    [Authorize(Roles = "Admin")]
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
    /// <param name="userId">Adresleri getirilecek kullanıcının ID'si.</param>
    /// <param name="pageIndex">Sayfa indeksi (varsayılan: 0).</param>
    /// <param name="pageSize">Sayfa boyutu (varsayılan: 20).</param>
    /// <returns>Kullanıcının tüm adresleri (sayfalanmış).</returns>
    /// <response code="200">Kullanıcının adresleri başarıyla döndürüldü.</response>
    /// <response code="400">Geçersiz kullanıcı ID'si.</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için Admin yetkisi gerekli.</response>
    [HttpGet("admin/user/{userId}/all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IPaginate<GetByUserIdAddressResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserAllAddresses(
        [FromRoute] string userId,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("Kullanıcı ID'si boş olamaz.");
        }

        var query = new GetByUserIdAddressQuery
        {
            UserId = userId,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion
}