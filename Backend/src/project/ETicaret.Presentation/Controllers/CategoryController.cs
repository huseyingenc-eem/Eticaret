using ETicaret.Application.Features.Categories.Commands.Create;
using ETicaret.Application.Features.Categories.Commands.Delete;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Application.Features.Categories.Queries.GetCategoryTree;
using ETicaret.Application.Features.Categories.Queries.GetChildCategories;
using ETicaret.Application.Features.Categories.Queries.GetParentCategories;
using MediatR;
using ETicaret.Presentation.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.Application.Abstractions.Services;
using System.Security.Claims;

namespace ETicaret.Presentation.Controllers;

public class CategoryController : BaseApiController
{
    private readonly IAuthorizationRuleService _authorizationRuleService;
    private readonly ILoggerService _loggerService;

    public CategoryController(
        IMediator mediator,
        IAuthorizationRuleService authorizationRuleService,
        ILoggerService loggerService) : base(mediator)
    {
        _authorizationRuleService = authorizationRuleService;
        _loggerService = loggerService;
    }

    /// <summary>
    /// Yeni kategori oluşturur.
    /// Dinamik authorization: CreateCategoryCommand operasyonu için gerekli roller kontrol edilir.
    /// </summary>
    [HttpPost("add")]
    [Authorize] // Önce kimlik doğrulama gerekli
    public async Task<IActionResult> Add([FromBody] CreateCategoryCommand command)
    {
        // 🎯 Dinamik authorization kontrolü
        await CheckOperationAuthorizationAsync(nameof(CreateCategoryCommand));

        var result = await _mediator.Send(command);
        return Created("", result);
    }

    /// <summary>
    /// Kategoriyi siler.
    /// Dinamik authorization: DeleteCategoryCommand operasyonu için gerekli roller kontrol edilir.
    /// </summary>
    [HttpDelete("delete/{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        // 🎯 Dinamik authorization kontrolü
        await CheckOperationAuthorizationAsync(nameof(DeleteCategoryCommand));

        DeleteCategoryCommand command = new() { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Kategoriyi günceller.
    /// Dinamik authorization: UpdateCategoryCommand operasyonu için gerekli roller kontrol edilir.
    /// </summary>
    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update(UpdateCategoryCommand command)
    {
        // 🎯 Dinamik authorization kontrolü
        await CheckOperationAuthorizationAsync(nameof(UpdateCategoryCommand));

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Tüm kategorileri ağaç yapısı (parent-child) şeklinde getirir.
    /// Bu işlem herkes tarafından erişilebilir (IPublicRequest).
    /// </summary>
    [HttpGet("GetCategoryTree")]
    public async Task<IActionResult> GetCategoryTree()
    {
        // 🔓 IPublicRequest sayesinde authorization behavior'ı atlanır
        var query = new GetCategoryTreeQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Belirli bir üst kategoriye ait alt kategorileri getirir.
    /// Bu işlem herkes tarafından erişilebilir (IPublicRequest).
    /// </summary>
    [HttpGet("GetChildCategories/{parentId}")]
    public async Task<IActionResult> GetChildCategories([FromRoute] int parentId)
    {
        // 🔓 IPublicRequest sayesinde authorization behavior'ı atlanır
        var query = new GetChildCategoriesQuery { ParentId = parentId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Tüm üst (ana) kategorileri getirir.
    /// Bu işlem herkes tarafından erişilebilir (IPublicRequest).
    /// </summary>
    [HttpGet("GetParentCategories")]
    public async Task<IActionResult> GetParentCategories()
    {
        // 🔓 IPublicRequest sayesinde authorization behavior'ı atlanır
        var query = new GetParentCategoriesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 🎯 Dinamik authorization kontrol metodu.
    /// Veritabanından operasyon için gerekli rolleri alır ve kullanıcının yetkisini kontrol eder.
    /// </summary>
    /// <param name="operationName">Kontrol edilecek operasyon adı (örn: "CreateCategoryCommand")</param>
    private async Task CheckOperationAuthorizationAsync(string operationName)
    {
        try
        {
            // 1. Operasyon için gerekli rolleri veritabanından al
            var requiredRoles = await _authorizationRuleService.GetRequiredRolesAsync(operationName);

            // 2. Eğer hiç rol gerekmiyor ise, herkese açık operasyon
            if (requiredRoles == null || !requiredRoles.Any())
            {
                _loggerService.Info($"Operation '{operationName}' is public, no authorization required.");
                return; // Yetki kontrolü gerekmiyor
            }

            // 3. Kullanıcının rollerini JWT token'dan al
            var userRoles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            // 4. Kullanıcının hiç rolü yoksa yetkisiz
            if (!userRoles.Any())
            {
                _loggerService.Warning($"User has no roles for operation '{operationName}'");
                throw new Core.Application.Common.Exceptions.AuthorizationException(
                    message: $"No roles found for operation '{operationName}'",
                    userFriendlyMessage: "Bu işlem için yetkiniz bulunmuyor.",
                    errorCode: "NO_ROLES"
                );
            }

            // 5. Kullanıcının rollerinden herhangi biri gerekli rollerden biriyle eşleşiyor mu?
            bool hasPermission = requiredRoles.Any(requiredRole =>
                userRoles.Contains(requiredRole, StringComparer.OrdinalIgnoreCase));

            if (!hasPermission)
            {
                _loggerService.Warning($"Authorization failed for operation '{operationName}'. Required: [{string.Join(", ", requiredRoles)}], User has: [{string.Join(", ", userRoles)}]");

                throw new Core.Application.Common.Exceptions.AuthorizationException(
                    message: $"Insufficient permissions for operation '{operationName}'. Required roles: {string.Join(", ", requiredRoles)}",
                    userFriendlyMessage: "Bu işlem için yetkiniz bulunmuyor.",
                    errorCode: "INSUFFICIENT_PERMISSIONS",
                    additionalData: new
                    {
                        Operation = operationName,
                        RequiredRoles = requiredRoles,
                        UserRoles = userRoles
                    }
                );
            }

            _loggerService.Info($"Authorization successful for operation '{operationName}'. User roles: [{string.Join(", ", userRoles)}]");
        }
        catch (Core.Application.Common.Exceptions.AuthorizationException)
        {
            // Authorization hatalarını yeniden fırlat
            throw;
        }
        catch (Exception ex)
        {
            _loggerService.Error($"Error during authorization check for operation '{operationName}': {ex.Message}", ex);

            throw new Core.Application.Common.Exceptions.AuthorizationException(
                message: $"Authorization check failed for operation '{operationName}': {ex.Message}",
                userFriendlyMessage: "Yetki kontrolü sırasında bir hata oluştu.",
                errorCode: "AUTHORIZATION_CHECK_FAILED"
            );
        }
    }
}