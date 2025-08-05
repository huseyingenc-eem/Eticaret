using ETicaret.Application.Features.Discounts.Commands.Create;
using ETicaret.Application.Features.Discounts.Commands.Update;
using ETicaret.Application.Features.Discounts.Commands.Delete;
using ETicaret.Application.Features.Discounts.Queries.GetById;
using ETicaret.Application.Features.Discounts.Queries.GetList;
using ETicaret.Application.Features.Discounts.Queries.GetByCode;
using ETicaret.Application.Features.Discounts.Queries.GetActiveList;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

/// <summary>
/// İndirim yönetimi için API endpoint'lerini sağlayan controller.
/// Admin işlemleri için yetkilendirme gerekli, query işlemleri herkese açık.
/// </summary>
public class DiscountsController : BaseApiController
{
    public DiscountsController(IMediator mediator) : base(mediator)
    {
    }

    #region Query Endpoints (Public - Herkes Erişebilir)

    /// <summary>
    /// Tüm indirimleri sayfalanmış şekilde getirir. Filtreleme ve önbellekleme desteği ile.
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] GetListDiscountQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip indirimi getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var query = new GetByIdDiscountQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen kupon koduna sahip aktif indirimi getirir.
    /// Müşterilerin kupon kodu ile indirim araması için.
    /// </summary>
    [HttpGet("code/{discountCode}")]
    public async Task<IActionResult> GetByCode([FromRoute] string discountCode)
    {
        var query = new GetByCodeDiscountQuery { DiscountCode = discountCode };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Şu anda aktif olan tüm indirimleri getirir.
    /// Müşteri arayüzü için optimize edilmiş.
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveDiscounts()
    {
        var query = new GetActiveDiscountQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Command Endpoints (Admin Only - Yetkilendirme Gerekli)

    /// <summary>
    /// Yeni bir indirim oluşturur. Sadece Admin kullanıcıları erişebilir.
    /// Rules Engine otomatik olarak business rule'ları kontrol eder.
    /// </summary>
    [HttpPost("create")]
    [Authorize] // Rules Engine'de [DefaultRoles("Admin")] tanımlı
    public async Task<IActionResult> Create([FromBody] CreateDiscountCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Mevcut bir indirimi günceller. Sadece Admin kullanıcıları erişebilir.
    /// Rules Engine otomatik olarak business rule'ları kontrol eder.
    /// </summary>
    [HttpPut("update")]
    [Authorize] // Rules Engine'de [DefaultRoles("Admin")] tanımlı
    public async Task<IActionResult> Update([FromBody] UpdateDiscountCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Mevcut bir indirimi siler (soft delete). Sadece Admin kullanıcıları erişebilir.
    /// Rules Engine otomatik olarak business rule'ları kontrol eder.
    /// </summary>
    [HttpDelete("delete/{id:guid}")]
    [Authorize] // Rules Engine'de [DefaultRoles("Admin")] tanımlı
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteDiscountCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

   
}
