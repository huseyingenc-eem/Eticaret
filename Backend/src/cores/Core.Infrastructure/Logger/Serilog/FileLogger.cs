using Core.Infrastructure.Logger.Serilog.ConfigurationModels;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Core.CrossCuttingConcerns.Logger.Serilog;

public class FileLogger : LoggerServiceBase
{
    private readonly IConfiguration _configuration;

    public FileLogger(IConfiguration configuration)
    {
        _configuration = configuration;

        FileLogConfiguration logConfiguration = configuration.GetSection("SerilogLogConfigurations:FileLogConfiguration").Get<FileLogConfiguration>();

        // İki klasör yukarı çık
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string twoLevelsUpDirectory = Path.GetDirectoryName(Path.GetDirectoryName(baseDirectory));

        // ETicaret.Presentation klasörünün yolunu oluştur
        string presentationDirectory = Path.Combine(twoLevelsUpDirectory, "..", "..");

        // Log dosyasının yolunu oluştur
        string logFolderPath = Path.Combine(presentationDirectory, logConfiguration.FolderPath);

        // Eğer klasör yoksa oluştur
        if (!Directory.Exists(logFolderPath))
        {
            Directory.CreateDirectory(logFolderPath);
        }

        // Dosya adını ve yolunu oluştur
        string filepath = Path.Combine(logFolderPath, DateTime.Now.ToString("yyyy-MM-dd") + logConfiguration.FileExtension);

        Logger = new LoggerConfiguration()
            .WriteTo.File(
                filepath,
                rollingInterval: RollingInterval.Day,
                retainedFileTimeLimit: null,
                fileSizeLimitBytes: logConfiguration.FileSizeLimitBytes,
                outputTemplate: logConfiguration.OutputTemplate
            )
            .CreateLogger();
    }
}
