namespace ETicaret.Persistence.Diagnostics;

public sealed class SqlCommandLoggingOptions
{
    /// <summary>Yavaş sorgu eşiği (ms)</summary>
    public int SlowThresholdMs { get; set; } = 100;

    /// <summary>Parametreleri logla (sadece Development için önerilir)</summary>
    public bool IncludeParameters { get; set; } = true;

    /// <summary>Uzun SQL’leri kısalt</summary>
    public int MaxSqlChars { get; set; } = 4000;

    /// <summary>Konsolda ANSI renkleri kullan</summary>
    public bool EnableAnsiColor { get; set; } = true;

    /// <summary>Basit SQL biçimlendirme (satır kırma vb.)</summary>
    public bool PrettyFormatSql { get; set; } = true;

    /// <summary>Çalıştırmadan önce (Executing) kısa debug satırı yaz</summary>
    public bool LogBeforeExecute { get; set; } = false;

    /// <summary>Logger’a ek olarak Console.WriteLine ile de yaz</summary>
    public bool EchoToConsole { get; set; } = true;
}
