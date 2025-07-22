using Core.Application.Abstractions.Messaging;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ETicaret.Presentation.Middlewares;

/// <summary>
/// HTTP isteğinin body'sindeki komut nesnesini okuyarak, eğer komut IAuthenticatedRequest
/// arayüzünü uyguluyorsa, JWT token'dan gelen kullanıcı kimliğini (UserId) otomatik olarak atar.
/// </summary>
public class AssignUserIdMiddleware
{
    private readonly RequestDelegate _next;

    public AssignUserIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Kullanıcı kimliği doğrulanmamışsa veya istekte body yoksa, hiçbir şey yapma.
        if (context.User.Identity?.IsAuthenticated != true || context.Request.ContentLength == null || context.Request.ContentLength == 0)
        {
            await _next(context);
            return;
        }

        // 2. İstek body'sinin tekrar okunabilmesi için buffering'i etkinleştir. Bu çok önemli!
        context.Request.EnableBuffering();

        // 3. Kullanıcının kimliğini (UserId) taleplerden (claims) al.
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            await _next(context);
            return;
        }

        // 4. İstek body'sini oku ve bir JSON nesnesine dönüştür.
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var bodyAsString = await reader.ReadToEndAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var command = JsonSerializer.Deserialize<IAuthenticatedRequest>(bodyAsString, options);

        // 5. Eğer nesne IAuthenticatedRequest ise ve UserId'si boşsa, ata.
        if (command != null && string.IsNullOrEmpty(command.UserId))
        {
            command.UserId = userId;

            // 6. Değiştirilmiş nesneyi tekrar JSON string'ine çevir.
            var updatedBodyAsString = JsonSerializer.Serialize(command, command.GetType(), options);
            var updatedBodyBytes = Encoding.UTF8.GetBytes(updatedBodyAsString);

            // 7. Değiştirilmiş body ile yeni bir stream oluştur ve isteğin body'si olarak ata.
            var memoryStream = new MemoryStream(updatedBodyBytes);
            context.Request.Body = memoryStream;
            context.Request.ContentLength = memoryStream.Length;
        }

        // 8. İsteğin body stream'ini başa sar, böylece sonraki middleware/controller onu okuyabilir.
        context.Request.Body.Position = 0;

        // 9. Pipeline'daki bir sonraki adıma geç.
        await _next(context);
    }
}