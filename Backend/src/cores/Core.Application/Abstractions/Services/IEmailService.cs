namespace Core.Application.Abstractions.Services;

public class EmailMessage
{
    public List<string> To { get; set; } = new();
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsBodyHtml { get; set; } = true;
}

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage emailMessage);
}