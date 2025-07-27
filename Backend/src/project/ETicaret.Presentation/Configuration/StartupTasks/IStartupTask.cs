namespace ETicaret.Presentation.Configuration.StartupTasks;

/// <summary>
/// Uygulama başlangıcında çalışacak görevler için arayüz.
/// </summary>
public interface IStartupTask
{
    /// <summary>
    /// Görevin öncelik sırası.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Startup görevini çalıştırır.
    /// </summary>
    /// <param name="host">Web application host.</param>
    Task ExecuteAsync(IHost host);
}