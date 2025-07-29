using Core.Application.Common.Results;
using ETicaret.Application.Features.UserRoles.Commands.Create;
using ETicaret.Application.Features.UserRoles.Commands.Delete;
using ETicaret.Application.Features.UserRoles.Commands.Update;
using ETicaret.Application.Features.UserRoles.Queries.GetList;
using ETicaret.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Presentation.Controllers;

/// <summary>
/// Kullanıcı-rol yönetimi için API endpoint'lerini sağlayan controller.
/// Kullanıcılara rol atama, kaldırma, güncelleme ve listeleme işlemlerini yönetir.
/// </summary>
[Authorize] // Tüm işlemler için kimlik doğrulama gerekli
public class UserRolesController : BaseApiController
{
    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// UserRolesController sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="mediator">MediatR aracı için dependency injection.</param>
    public UserRolesController(IMediator mediator) : base(mediator)
    {
    }

    #endregion

    #region Kullanıcı-Rol Listeleme (Get User Roles List)

    /// <summary>
    /// Tüm kullanıcıları ve onlara atanmış rolleri sayfalanmış bir şekilde getirir.
    /// Arama, filtreleme ve sıralama desteği sağlar.
    /// </summary>
    /// <param name="query">Liste sorgusu parametreleri (sayfa, boyut, filtreler).</param>
    /// <returns>Kullanıcı-rol listesi (sayfalanmış).</returns>
    /// <response code="200">Kullanıcı-rol listesi başarıyla döndürüldü.</response>
    /// <response code="400">Geçersiz sorgu parametreleri (validation hatası).</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    [HttpGet("list")]
    [ProducesResponseType(typeof(PagedResult<GetListUserRolesResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserRolesList([FromQuery] GetListUserRolesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirli bir kullanıcının sahip olduğu rolleri getirir.
    /// </summary>
    /// <param name="userId">Rolleri getirilecek kullanıcının ID'si.</param>
    /// <returns>Kullanıcının rol listesi.</returns>
    /// <response code="200">Kullanıcının rolleri başarıyla döndürüldü.</response>
    /// <response code="400">Geçersiz kullanıcı ID'si.</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    /// <response code="404">Kullanıcı bulunamadı.</response>
    [HttpGet("user/{userId}/roles")]
    [ProducesResponseType(typeof(List<UserRoleDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRoles([FromRoute] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("Kullanıcı ID'si boş olamaz.");
        }

        // Sadece belirli bir kullanıcının rollerini getirmek için filtrelenmiş sorgu
        var query = new GetListUserRolesQuery
        {
            PageIndex = 0,
            PageSize = 1,
            SearchTerm = userId // UserId ile arama yaparak tek kullanıcıyı getir
        };

        var result = await _mediator.Send(query);

        if (!result.Items.Any())
        {
            return NotFound("Kullanıcı bulunamadı.");
        }

        var userRoles = result.Items.First().Roles;
        return Ok(userRoles);
    }

    #endregion

    #region Kullanıcıya Rol Atama (Assign Role to User)

    /// <summary>
    /// Belirli bir kullanıcıya yeni bir rol atar.
    /// </summary>
    /// <param name="command">Rol atama komutu (kullanıcı ID'si ve rol ID'si).</param>
    /// <returns>Atama işleminin sonucu.</returns>
    /// <response code="201">Rol başarıyla kullanıcıya atandı.</response>
    /// <response code="400">Geçersiz istek verisi (validation hatası).</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    /// <response code="404">Kullanıcı veya rol bulunamadı.</response>
    /// <response code="409">Kullanıcı zaten bu role sahip.</response>
    [HttpPost("assign")]
    [ProducesResponseType(typeof(CreateUserRolesResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignRoleToUser([FromBody] CreateUserRolesCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetUserRoles),
            new { userId = command.UserId },
            result
        );
    }

    /// <summary>
    /// Bir kullanıcıya toplu olarak birden fazla rol atar.
    /// </summary>
    /// <param name="command">Toplu rol atama komutu.</param>
    /// <returns>Toplu atama işleminin sonucu.</returns>
    /// <response code="200">Roller başarıyla kullanıcıya atandı.</response>
    /// <response code="400">Geçersiz istek verisi.</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    /// <response code="404">Kullanıcı veya rol(ler) bulunamadı.</response>
    [HttpPost("assign-multiple")]
    [ProducesResponseType(typeof(UpdateUserRolesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignMultipleRolesToUser([FromBody] UpdateUserRolesCommand command)
    {
        // RolesToRemove listesini boşalt, sadece ekleme işlemi yap
        command.RolesToRemove = new List<string>();

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Kullanıcı Rollerini Güncelleme (Update User Roles)

    /// <summary>
    /// Kullanıcının rollerini günceller (ekleme ve/veya kaldırma).
    /// Aynı istekte hem rol ekleyebilir hem de kaldırabilirsiniz.
    /// </summary>
    /// <param name="userId">Rolleri güncellenecek kullanıcının ID'si.</param>
    /// <param name="command">Güncelleme komutu (eklenecek ve kaldırılacak roller).</param>
    /// <returns>Güncelleme işleminin sonucu.</returns>
    /// <response code="200">Kullanıcı rolleri başarıyla güncellendi.</response>
    /// <response code="400">Geçersiz istek verisi (validation hatası).</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    /// <response code="404">Kullanıcı veya rol(ler) bulunamadı.</response>
    [HttpPut("user/{userId}/roles")]
    [ProducesResponseType(typeof(UpdateUserRolesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserRoles(
        [FromRoute] string userId,
        [FromBody] UpdateUserRolesCommand command)
    {
        // Route'dan gelen UserId'yi command'a ata
        command.UserId = userId;

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcının mevcut rollerini tamamen yenileri ile değiştirir.
    /// Kullanıcının tüm rolleri kaldırılır ve yeni roller atanır.
    /// </summary>
    /// <param name="userId">Rolleri değiştirilecek kullanıcının ID'si.</param>
    /// <param name="request">Yeni rol ID'leri listesi.</param>
    /// <returns>Değiştirme işleminin sonucu.</returns>
    /// <response code="200">Kullanıcı rolleri başarıyla değiştirildi.</response>
    /// <response code="400">Geçersiz istek verisi.</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    /// <response code="404">Kullanıcı veya rol(ler) bulunamadı.</response>
    [HttpPut("user/{userId}/roles/replace")]
    [ProducesResponseType(typeof(UpdateUserRolesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReplaceUserRoles(
        [FromRoute] string userId,
        [FromBody] ReplaceUserRolesRequest request)
    {
        // Önce mevcut rolleri al
        var currentRolesQuery = new GetListUserRolesQuery
        {
            PageIndex = 0,
            PageSize = 1,
            SearchTerm = userId
        };

        var currentUserResult = await _mediator.Send(currentRolesQuery);

        if (!currentUserResult.Items.Any())
        {
            return NotFound("Kullanıcı bulunamadı.");
        }

        var currentRoleIds = currentUserResult.Items.First().Roles.Select(r => r.RoleId).ToList();

        // Güncelleme komutunu hazırla
        var command = new UpdateUserRolesCommand
        {
            UserId = userId,
            RolesToAdd = request.NewRoleIds ?? new List<string>(),
            RolesToRemove = currentRoleIds
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Kullanıcıdan Rol Kaldırma (Remove Role from User)

    /// <summary>
    /// Kullanıcıdan belirli bir rolü kaldırır.
    /// </summary>
    /// <param name="userId">Rolü kaldırılacak kullanıcının ID'si.</param>
    /// <param name="roleId">Kaldırılacak rolün ID'si.</param>
    /// <returns>Kaldırma işleminin sonucu.</returns>
    /// <response code="200">Rol başarıyla kullanıcıdan kaldırıldı.</response>
    /// <response code="400">Geçersiz kullanıcı veya rol ID'si.</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    /// <response code="404">Kullanıcı veya rol bulunamadı.</response>
    /// <response code="409">Son rol kaldırılamaz veya kritik rol.</response>
    [HttpDelete("user/{userId}/role/{roleId}")]
    [ProducesResponseType(typeof(DeleteUserRoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveRoleFromUser(
        [FromRoute] string userId,
        [FromRoute] string roleId)
    {
        var command = new DeleteUserRoleCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcıdan birden fazla rolü toplu olarak kaldırır.
    /// </summary>
    /// <param name="userId">Rolleri kaldırılacak kullanıcının ID'si.</param>
    /// <param name="request">Kaldırılacak rol ID'leri listesi.</param>
    /// <returns>Toplu kaldırma işleminin sonucu.</returns>
    /// <response code="200">Roller başarıyla kullanıcıdan kaldırıldı.</response>
    /// <response code="400">Geçersiz istek verisi.</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    /// <response code="404">Kullanıcı veya rol(ler) bulunamadı.</response>
    [HttpDelete("user/{userId}/roles/multiple")]
    [ProducesResponseType(typeof(UpdateUserRolesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMultipleRolesFromUser(
        [FromRoute] string userId,
        [FromBody] RemoveMultipleRolesRequest request)
    {
        var command = new UpdateUserRolesCommand
        {
            UserId = userId,
            RolesToAdd = new List<string>(), // Ekleme yok
            RolesToRemove = request.RoleIdsToRemove ?? new List<string>()
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Yardımcı Endpoint'ler (Helper Endpoints)

    /// <summary>
    /// Belirli bir role sahip tüm kullanıcıları listeler.
    /// </summary>
    /// <param name="roleName">Aranacak rol adı.</param>
    /// <param name="pageIndex">Sayfa indeksi (varsayılan: 0).</param>
    /// <param name="pageSize">Sayfa boyutu (varsayılan: 10).</param>
    /// <returns>Role sahip kullanıcıların listesi.</returns>
    /// <response code="200">Role sahip kullanıcılar başarıyla döndürüldü.</response>
    /// <response code="400">Geçersiz rol adı.</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    [HttpGet("role/{roleName}/users")]
    [ProducesResponseType(typeof(PagedResult<GetListUserRolesResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsersByRole(
        [FromRoute] string roleName,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return BadRequest("Rol adı boş olamaz.");
        }

        var query = new GetListUserRolesQuery
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            RoleFilter = roleName
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Sistem genelinde rol dağılımı istatistiklerini getirir.
    /// </summary>
    /// <returns>Rol istatistikleri.</returns>
    /// <response code="200">İstatistikler başarıyla döndürüldü.</response>
    /// <response code="401">Kimlik doğrulama başarısız.</response>
    /// <response code="403">Bu işlem için yetki yok.</response>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(UserRoleStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserRoleStatistics()
    {
        // Tüm kullanıcıları getir (büyük bir limit ile)
        var query = new GetListUserRolesQuery
        {
            PageIndex = 0,
            PageSize = 1000 // İstatistik için büyük sayfa boyutu
        };

        var result = await _mediator.Send(query);

        // İstatistikleri hesapla
        var statistics = new UserRoleStatisticsDto
        {
            TotalUsers = result.Count,
            TotalActiveUsers = result.Items.Count(u => u.EmailConfirmed),
            RoleDistribution = result.Items
                .SelectMany(u => u.Roles)
                .GroupBy(r => r.RoleName)
                .ToDictionary(g => g.Key, g => g.Count()),
            UsersWithMultipleRoles = result.Items.Count(u => u.RoleCount > 1),
            UsersWithNoRoles = result.Items.Count(u => u.RoleCount == 0)
        };

        return Ok(statistics);
    }

    #endregion
}

#region Yardımcı DTO'lar (Helper DTOs)

/// <summary>
/// Kullanıcının rollerini tamamen değiştirmek için kullanılan request DTO.
/// </summary>
public class ReplaceUserRolesRequest
{
    /// <summary>
    /// Kullanıcıya atanacak yeni rol ID'leri listesi.
    /// </summary>
    public List<string>? NewRoleIds { get; set; }
}

/// <summary>
/// Kullanıcıdan birden fazla rol kaldırmak için kullanılan request DTO.
/// </summary>
public class RemoveMultipleRolesRequest
{
    /// <summary>
    /// Kaldırılacak rol ID'leri listesi.
    /// </summary>
    public List<string>? RoleIdsToRemove { get; set; }
}

/// <summary>
/// Kullanıcı-rol istatistikleri için response DTO.
/// </summary>
public class UserRoleStatisticsDto
{
    /// <summary>
    /// Toplam kullanıcı sayısı.
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Aktif kullanıcı sayısı (email onaylanmış).
    /// </summary>
    public int TotalActiveUsers { get; set; }

    /// <summary>
    /// Rol dağılımı (rol adı : kullanıcı sayısı).
    /// </summary>
    public Dictionary<string, int> RoleDistribution { get; set; } = new();

    /// <summary>
    /// Birden fazla role sahip kullanıcı sayısı.
    /// </summary>
    public int UsersWithMultipleRoles { get; set; }

    /// <summary>
    /// Hiçbir role sahip olmayan kullanıcı sayısı.
    /// </summary>
    public int UsersWithNoRoles { get; set; }
}

#endregion