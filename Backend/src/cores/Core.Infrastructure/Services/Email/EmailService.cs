using Core.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Services.Email;

/// <summary>
/// E-posta gönderme işlemlerini yürüten somut servis.
/// Bu örnek, loglama üzerinden simülasyon yapar. Gerçek bir projede
/// MailKit, SendGrid gibi bir sağlayıcı entegre edilir.
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(EmailMessage emailMessage)
    {
        // Gerçek e-posta gönderme mantığı burada yer alır.
        // Örnek olarak, gönderilecek e-postayı logluyoruz.
        _logger.LogInformation(
            "E-posta gönderiliyor: Kime={To}, Konu={Subject}, HTML mi={IsHtml}",
            string.Join(",", emailMessage.To),
            emailMessage.Subject,
            emailMessage.IsBodyHtml
        );

        // Bu sadece bir simülasyon olduğu için tamamlanmış bir Task dönüyoruz.
        return Task.CompletedTask;
    }
}