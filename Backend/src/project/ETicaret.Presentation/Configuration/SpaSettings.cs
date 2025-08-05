namespace ETicaret.Presentation.Configuration;

public class SpaSettings
{
    public const string SectionName = "SpaSettings";

    public bool Enabled { get; set; } = true;
    public string RootPath { get; set; } = string.Empty;
    public string? StartupCommand { get; set; }
    public string? ServerUrl { get; set; }
    public string ApiBasePath { get; set; } = "/api";
    public bool HotReload { get; set; } = true;
}