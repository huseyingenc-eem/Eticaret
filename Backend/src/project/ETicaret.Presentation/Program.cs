using ETicaret.Presentation.Extensions;
using ETicaret.Presentation.Configuration;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Host yapılandırması (Serilog)
    builder.ConfigureHost();

    // SPA ayarlarını yapılandır
    var spaSettings = builder.Configuration.GetSection(SpaSettings.SectionName).Get<SpaSettings>() ?? new SpaSettings();

    // Ortam değişkeninden SPA durumunu kontrol et
    var spaEnabledOverride = builder.Configuration["SPA_ENABLED"];
    if (!string.IsNullOrEmpty(spaEnabledOverride))
    {
        spaSettings.Enabled = bool.Parse(spaEnabledOverride);
    }

    // Tüm servisleri yapılandır
    builder.Services.AddConfiguredServices(builder.Configuration, builder.Environment);

    // SPA servislerini ekle
    if (spaSettings.Enabled && builder.Environment.IsDevelopment())
    {
        builder.Services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = Path.Combine(spaSettings.RootPath, "dist");
        });
    }

    var app = builder.Build();

    // Middleware'leri yapılandır
    app.UseConfiguredMiddlewares();

    // SPA middleware'lerini yapılandır
    if (spaSettings.Enabled)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseSpaStaticFiles();
        }

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = spaSettings.RootPath;

            if (app.Environment.IsDevelopment())
            {
                if (!string.IsNullOrEmpty(spaSettings.StartupCommand) &&
                    !string.IsNullOrEmpty(spaSettings.ServerUrl))
                {
                    spa.UseProxyToSpaDevelopmentServer(spaSettings.ServerUrl);
                }
            }
        });
    }

    // Startup task'ları çalıştır
    await app.RunStartupTasksAsync();

    Console.WriteLine($"Application started successfully! SPA Enabled: {spaSettings.Enabled}");

    if (spaSettings.Enabled && app.Environment.IsDevelopment())
    {
        Console.WriteLine($"SPA Development Server: {spaSettings.ServerUrl}");
        Console.WriteLine($"React Client Path: {spaSettings.RootPath}");
    }

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup failed: {ex}");
    throw;
}