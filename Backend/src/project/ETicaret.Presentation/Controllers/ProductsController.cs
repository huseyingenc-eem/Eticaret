using Core.Application.Common.Results;
using ETicaret.Application.Features.Products.Commands.Create;
using ETicaret.Application.Features.Products.Commands.Delete;
using ETicaret.Application.Features.Products.Commands.Update;
using ETicaret.Application.Features.Products.Queries.GetById;
using ETicaret.Application.Features.Products.Queries.GetList;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

/// <summary>
/// Ürün yönetimi için API endpoint'lerini sağlayan controller.
/// Admin işlemleri için yetkilendirme gerekli, query işlemleri kısmen herkese açık.
/// </summary>
public class ProductsController : BaseApiController
{
    public ProductsController(IMediator mediator) : base(mediator)
    {
    }

    #region Query Endpoints (Public/Protected)

    /// <summary>
    /// Tüm ürünleri sayfalanmış şekilde getirir. Filtreleme ve önbellekleme desteği ile.
    /// Herkese açık - ürün katalog sayfası için kullanılır.
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(PagedResult<GetListProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetList([FromQuery] GetListProductQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip ürünü tüm detaylarıyla getirir.
    /// Ürün detay sayfası için kullanılır - herkese açık.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetByIdProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var query = new GetByIdProductQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    

    /// <summary>
    /// Ürün arama işlemi - isim, açıklama gibi alanlarda arama yapar.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<GetListProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
    {
        var query = new GetListProductQuery
        {
            NameSearch = searchTerm,
            PageIndex = pageIndex,
            PageSize = pageSize,
            OnlyActive = true
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Command Endpoints (Admin Only)

    /// <summary>
    /// Yeni bir ürün oluşturur. Sadece Admin kullanıcıları erişebilir.
    /// Rules Engine otomatik olarak business rule'ları kontrol eder.
    /// </summary>
    [HttpPost("create")]
    [Authorize] // Rules Engine'de [DefaultRoles("Admin")] tanımlı
    [ProducesResponseType(typeof(CreateProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)] // Name already exists
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Mevcut bir ürünü günceller. Sadece Admin kullanıcıları erişebilir.
    /// Rules Engine otomatik olarak business rule'ları kontrol eder.
    /// </summary>
    [HttpPut("update")]
    [Authorize] // Rules Engine'de [DefaultRoles("Admin")] tanımlı
    [ProducesResponseType(typeof(UpdateProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)] // Name conflict
    public async Task<IActionResult> Update([FromBody] UpdateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Mevcut bir ürünü siler (soft delete varsayılan). Sadece Admin kullanıcıları erişebilir.
    /// Rules Engine otomatik olarak business rule'ları kontrol eder.
    /// </summary>
    [HttpDelete("delete/{id:guid}")]
    [Authorize] // Rules Engine'de [DefaultRoles("Admin")] tanımlı
    [ProducesResponseType(typeof(DeleteProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)] // Has variants or in use
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteProductCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Admin Analytics & Management

    /// <summary>
    /// Admin dashboard için ürün istatistikleri.
    /// </summary>
    [HttpGet("admin/stats")]
    [Authorize] // Rules Engine'de [DefaultRoles("Admin")] tanımlı
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProductStats()
    {
        var totalQuery = new GetListProductQuery { PageSize = 1, OnlyActive = false };
        var activeQuery = new GetListProductQuery { PageSize = 1, OnlyActive = true };

        var totalResult = await _mediator.Send(totalQuery);
        var activeResult = await _mediator.Send(activeQuery);

        return Ok(new
        {
            TotalProducts = totalResult.Count,
            ActiveProducts = activeResult.Count,
            InactiveProducts = totalResult.Count - activeResult.Count,
            LastUpdated = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Stoksuz ürünleri getirir (Admin).
    /// </summary>
    [HttpGet("admin/out-of-stock")]
    [Authorize] // Rules Engine'de [DefaultRoles("Admin")] tanımlı
    [ProducesResponseType(typeof(PagedResult<GetListProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOutOfStockProducts([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
    {
        // Bu query'i GetListProductQuery'yi extend ederek yapabiliriz
        var query = new GetListProductQuery
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            OnlyActive = true
            // OutOfStock filtering - bu özelliği GetListProductQuery'ye eklemek gerekebilir
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen tedarikçiye ait ürünleri getirir (Admin).
    /// </summary>
    [HttpGet("admin/supplier/{supplierId:guid}")]
    [Authorize] // Rules Engine'de [DefaultRoles("Admin")] tanımlı
    [ProducesResponseType(typeof(PagedResult<GetListProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetBySupplier([FromRoute] Guid supplierId, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
    {
        var query = new GetListProductQuery
        {
            SupplierId = supplierId,
            PageIndex = pageIndex,
            PageSize = pageSize,
            OnlyActive = false
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Bulk Operations

    ///// <summary>
    ///// Toplu ürün aktivasyon/deaktivasyon işlemi (Admin).
    ///// </summary>
    //[HttpPatch("admin/bulk-status")]
    //[Authorize]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status403Forbidden)]
    //public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateProductStatusCommand command)
    //{
    //    var result = await _mediator.Send(command);
    //    return Ok(result);
    //}

    #endregion
}