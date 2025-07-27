using ETicaret.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Host yapılandırması (Serilog)
    builder.ConfigureHost();

    // Tüm servisleri yapılandır
    builder.Services.AddConfiguredServices(builder.Configuration, builder.Environment);

    var app = builder.Build();

    // Middleware'leri yapılandır
    app.UseConfiguredMiddlewares();

    // Startup task'ları çalıştır
    await app.RunStartupTasksAsync();

    Console.WriteLine("Application started successfully!");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup failed: {ex}");
    throw;
}