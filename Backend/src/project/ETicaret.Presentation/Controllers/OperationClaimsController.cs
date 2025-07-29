using ETicaret.Application.Features.OperationClaims.Commands.Update;
using ETicaret.Application.Features.OperationClaims.Queries.GetList;
using ETicaret.Application.Services.Authorization;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

/// <summary>
/// Operasyon yetkileri yönetimi için API endpoint'leri.
/// Bu controller sadece Admin rolüne sahip kullanıcılar tarafından erişilebilir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class OperationClaimsController : BaseApiController
{
    public OperationClaimsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Tüm operasyon yetkilerini filtreli liste olarak getirir.
    /// </summary>
    /// <param name="query">Filtreleme parametreleri.</param>
    /// <returns>Operasyon yetkileri listesi.</returns>
    /// <response code="200">Operasyon yetkileri listesi başarıyla döndürüldü.</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    /// <response code="403">Bu işlem için Admin yetkisi gerekli.</response>
    [HttpGet("list")]
    [ProducesResponseType(typeof(List<GetListOperationClaimsResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetList([FromQuery] GetListOperationClaimsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    /// <summary>
    /// Mevcut bir operasyon yetkisini günceller.
    /// </summary>
    /// <param name="command">Güncellenecek operasyon yetkisi bilgileri.</param>
    /// <returns>Güncellenen operasyon yetkisinin detayları.</returns>
    /// <response code="200">Operasyon yetkisi başarıyla güncellendi.</response>
    /// <response code="400">Geçersiz istek verisi (Validation hatası).</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    /// <response code="403">Bu işlem için Admin yetkisi gerekli.</response>
    /// <response code="404">Güncellenecek operasyon yetkisi bulunamadı.</response>
    [HttpPut("update")]
    [ProducesResponseType(typeof(UpdateOperationClaimResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateOperationClaimCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Sistemdeki tüm Command ve Query sınıflarını tarayarak eksik operasyon yetkilerini otomatik olarak oluşturur.
    /// Bu endpoint sadece geliştirme ve initial setup aşamalarında kullanılmalıdır.
    /// </summary>
    /// <returns>Seeding işlem sonucu.</returns>
    /// <response code="200">Seeding işlemi başarıyla tamamlandı.</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    /// <response code="403">Bu işlem için Admin yetkisi gerekli.</response>
    [HttpPost("seed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SeedOperationClaims([FromServices] IOperationClaimSeeder seeder)
    {
        await seeder.SeedOperationClaimsAsync();
        return Ok(new { Message = "Operasyon yetkileri başarıyla güncellendi." });
    }

    /// <summary>
    /// Belirtilen Feature Name'e göre operasyon yetkilerini getirir.
    /// </summary>
    /// <param name="featureName">Feature adı (örn: Categories, Users, Products).</param>
    /// <returns>Belirtilen feature'a ait operasyon yetkileri.</returns>
    /// <response code="200">Feature'a ait operasyon yetkileri başarıyla döndürüldü.</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    /// <response code="403">Bu işlem için Admin yetkisi gerekli.</response>
    [HttpGet("by-feature/{featureName}")]
    [ProducesResponseType(typeof(List<GetListOperationClaimsResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByFeatureName([FromRoute] string featureName)
    {
        var query = new GetListOperationClaimsQuery
        {
            FeatureNameFilter = featureName
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen rol adına göre operasyon yetkilerini getirir.
    /// </summary>
    /// <param name="roleName">Rol adı (örn: Admin, User, Moderator).</param>
    /// <returns>Belirtilen role sahip operasyon yetkileri.</returns>
    /// <response code="200">Role sahip operasyon yetkileri başarıyla döndürüldü.</response>
    /// <response code="401">Kullanıcı kimliği doğrulanamadı.</response>
    /// <response code="403">Bu işlem için Admin yetkisi gerekli.</response>
    [HttpGet("by-role/{roleName}")]
    [ProducesResponseType(typeof(List<GetListOperationClaimsResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByRequiredRole([FromRoute] string roleName)
    {
        var query = new GetListOperationClaimsQuery
        {
            RequiredRoleFilter = roleName
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}