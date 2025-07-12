namespace Core.CrossCuttingConcerns.Logger;
public interface ILoggerService
{
    void Debug(string message);
    void Error(string message, Exception exception = null);
    void Fatal(string message, Exception exception = null);
    void Info(string message);
    void Warning(string message);
}