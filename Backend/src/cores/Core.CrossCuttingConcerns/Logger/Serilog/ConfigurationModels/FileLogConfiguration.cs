namespace Core.CrossCuttingConcerns.Logger.Serilog.ConfigurationModels;

public class FileLogConfiguration
{
    public string FolderPath { get; set; } = "Logs/FileLogs/";
    public string FileExtension { get; set; } = ".txt";
    public string RollingInterval { get; set; } = "Day";
    public int? FileSizeLimitBytes { get; set; } = 5000000;
    public int? RetainedFileTimeLimitDays { get; set; }
    public string OutputTemplate { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
}