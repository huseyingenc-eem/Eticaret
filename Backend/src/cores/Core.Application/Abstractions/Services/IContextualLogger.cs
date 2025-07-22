namespace Core.Application.Abstractions.Services;

/// <summary>
/// HTTP isteği gibi bağlamsal bilgileri loglara otomatik olarak ekleyen
/// yapısal loglama servisi için sözleşme.
/// </summary>
public interface IContextualLogger
{
    void Verbose(string messageTemplate, params object[]? propertyValues);
    void Debug(string messageTemplate, params object[]? propertyValues);
    void Information(string messageTemplate, params object[]? propertyValues);
    void Warning(string messageTemplate, params object[]? propertyValues);
    void Error(Exception? exception, string messageTemplate, params object[]? propertyValues);
    void Fatal(Exception? exception, string messageTemplate, params object[]? propertyValues);
}