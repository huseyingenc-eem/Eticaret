using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Application.Features.Addresses.Queries.GetListByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims; // ClaimsPrincipal ve ClaimTypes için
using System.Threading.Tasks;

namespace ETicaret.Presentation.Controllers;

/// <summary>
/// Kullanıcı adresleri ile ilgili API operasyonlarını yönetir.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// AddressesController sınıfının yeni bir örneğini başlatır.
    /// </summary>
    /// <param name="mediator">MediatR arayüzü.</param>
    public AddressesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Giriş yapmış kullanıcının tüm adreslerini listeler.
    /// </summary>
    /// <returns>Kullanıcının adreslerinin listesi.</returns>
    /// <response code="200">Kullanıcının adres listesi başarıyla döndürüldü.</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    [HttpGet("my-addresses")]
    [ProducesResponseType(typeof(List<GetListAddressResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetListByCurrentUser()
    {
        // UserId'yi string olarak al
        var userId = GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) // Guid.Empty yerine string kontrolü
            return Unauthorized("Kullanıcı kimliği alınamadı.");

        // Query'deki UserId'yi string olarak ata
        GetListByUserIdAddressQuery getListByUserIdAddressQuery = new() { UserId = userId };
        List<GetListAddressResponseDto> result = await _mediator.Send(getListByUserIdAddressQuery);
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
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var userId = GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) // Guid.Empty yerine string kontrolü
            return Unauthorized("Kullanıcı kimliği alınamadı.");

        // Query'deki UserId'yi string olarak ata
        GetByIdAddressQuery getByIdAddressQuery = new() { Id = id, UserId = userId };
        GetByIdAddressResponseDto result = await _mediator.Send(getByIdAddressQuery);
        return Ok(result);
    }

    /// <summary>
    /// Giriş yapmış kullanıcı için yeni bir adres ekler.
    /// </summary>
    /// <param name="addressAddCommand">Eklenecek adres bilgileri.</param>
    /// <returns>Eklenen adresin bilgileri.</returns>
    /// <response code="201">Adres başarıyla oluşturuldu.</response>
    /// <response code="400">Geçersiz istek verisi (Validation hatası).</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateAddressResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Add([FromBody] CreateAddressCommand addressAddCommand)
    {
        var userId = GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) // Guid.Empty yerine string kontrolü
            return Unauthorized("Kullanıcı kimliği alınamadı.");
        addressAddCommand.UserId = userId; // Komuttaki UserId'yi string olarak set et

        CreateAddressResponseDto result = await _mediator.Send(addressAddCommand);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Mevcut bir adresi günceller (Sadece kullanıcının kendi adresi ise).
    /// </summary>
    /// <param name="addressUpdateCommand">Güncellenecek adres bilgileri.</param>
    /// <returns>Güncellenen adresin bilgileri.</returns>
    /// <response code="200">Adres başarıyla güncellendi.</response>
    /// <response code="400">Geçersiz istek verisi (Validation hatası).</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    /// <response code="403">Kullanıcının bu adresi güncelleme yetkisi yok.</response>
    /// <response code="404">Güncellenecek adres bulunamadı.</response>
    [HttpPut]
    [ProducesResponseType(typeof(UpdateAddressResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateAddressCommand addressUpdateCommand)
    {
        var userId = GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) // Guid.Empty yerine string kontrolü
            return Unauthorized("Kullanıcı kimliği alınamadı.");
        addressUpdateCommand.UserId = userId; // Handler'da kontrol için UserId'yi string olarak gönder

        UpdateAddressResponseDto result = await _mediator.Send(addressUpdateCommand);
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
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var userId = GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) // Guid.Empty yerine string kontrolü
            return Unauthorized("Kullanıcı kimliği alınamadı.");

        // Komuttaki UserId'yi string olarak ata
        DeleteAddressCommand addressDeleteCommand = new() { Id = id, UserId = userId };
        DeleteAddressResponseDto result = await _mediator.Send(addressDeleteCommand);
        return Ok(result);
    }

    /// <summary>
    /// İstek yapan kullanıcının kimliğini (UserId) JWT token içerisindeki NameIdentifier claim'inden alır.
    /// </summary>
    /// <returns>Kullanıcı ID'si (string) veya null (eğer alınamazsa).</returns>
    private string? GetUserIdFromClaims() // Dönüş tipi string? olarak değiştirildi
    {
        // HttpContext.User üzerinden Claim'lere erişilir
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        // Claim varsa ve değeri boş değilse döndür, yoksa null döndür
        return userIdClaim?.Value;
    }
}
