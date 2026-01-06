using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ETicaret.Persistence.Diagnostics;

public sealed class SqlCommandLoggingInterceptor : DbCommandInterceptor
{
    private readonly ILogger<SqlCommandLoggingInterceptor> _logger;
    private readonly SqlCommandLoggingOptions _opt;
    private static readonly Regex _ws = new(@"\s+", RegexOptions.Compiled);
    private static int _queryCounter = 0;

    public SqlCommandLoggingInterceptor(
        ILogger<SqlCommandLoggingInterceptor> logger,
        SqlCommandLoggingOptions opt)
    {
        _logger = logger;
        _opt = opt ?? new SqlCommandLoggingOptions();

        var msg = "🚀 SqlCommandLoggingInterceptor initialized!";
        Console.WriteLine(Colorize(msg, ConsoleColor.Green));
        _logger.LogInformation("SqlCommandLoggingInterceptor created");
    }

    // -------------------- BEFORE (Executing) HOOKS --------------------
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        LogBefore("ReaderExecuting", command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        LogBefore("ReaderExecutingAsync", command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        LogBefore("ScalarExecuting", command);
        return base.ScalarExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        LogBefore("ScalarExecutingAsync", command);
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        LogBefore("NonQueryExecuting", command);
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        LogBefore("NonQueryExecutingAsync", command);
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    // -------------------- AFTER (Executed) HOOKS --------------------
    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        Log(command, eventData, $"Affected rows: {result}", "NonQuery");
        return base.NonQueryExecuted(command, eventData, result);
    }

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        var resultText = result switch
        {
            null => "null",
            string s when s.Length > 50 => $"{s[..47]}...",
            _ => result.ToString()
        };
        Log(command, eventData, $"Result: {resultText}", "Scalar");
        return base.ScalarExecuted(command, eventData, result);
    }

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        Log(command, eventData, "DataReader returned", "Reader");
        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        Log(command, eventData, $"Affected rows: {result}", "NonQueryAsync");
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
    {
        var resultText = result switch
        {
            null => "null",
            string s when s.Length > 50 => $"{s[..47]}...",
            _ => result.ToString()
        };
        Log(command, eventData, $"Result: {resultText}", "ScalarAsync");
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        Log(command, eventData, "DataReader returned", "ReaderAsync");
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    // -------------------- BEFORE LOG --------------------
    private void LogBefore(string operation, DbCommand cmd)
    {
        if (!_opt.LogBeforeExecute) return;

        var queryId = Interlocked.Increment(ref _queryCounter);
        var shortSql = GetShortSql(cmd.CommandText ?? "");
        var msg = $"⚡ [{queryId:D4}] {operation} → {shortSql}";

        Console.WriteLine(Colorize(msg, ConsoleColor.Cyan));
    }

    // -------------------- MAIN LOG METHOD --------------------
    private void Log(DbCommand cmd, CommandExecutedEventData ev, string? resultHint, string kind)
    {
        var queryId = Interlocked.Increment(ref _queryCounter);
        var ms = ev.Duration.TotalMilliseconds;
        var slow = ms >= _opt.SlowThresholdMs;
        var verySlow = ms >= (_opt.SlowThresholdMs * 3);

        // Log level belirleme
        var level = verySlow ? LogLevel.Error : (slow ? LogLevel.Warning : LogLevel.Information);

        // Başlık ve renkler
        var title = verySlow ? "🐌 VERY SLOW QUERY" : (slow ? "🐢 SLOW QUERY" : "⚡ SQL QUERY");
        var titleColor = verySlow ? ConsoleColor.Red : (slow ? ConsoleColor.Yellow : ConsoleColor.Green);

        // Connection ve transaction bilgileri
        var db = cmd.Connection?.Database ?? "Unknown";
        var conn = Short(ev.ConnectionId.ToString());
        var tx = cmd.Transaction is null ? "NoTx" : "InTx";
        var sql = cmd.CommandText ?? "";

        // SQL formatting
        if (_opt.PrettyFormatSql) sql = Pretty(sql);
        var originalLength = sql.Length;
        if (sql.Length > _opt.MaxSqlChars)
        {
            sql = sql[.._opt.MaxSqlChars] + $"... (total: {originalLength} chars)";
        }

        // Performance indicators
        var perfIcon = ms switch
        {
            < 10 => "🚀",
            < 50 => "✅",
            < 100 => "⚠️",
            < 500 => "🐌",
            _ => "💀"
        };

        // Box çizimi
        var sb = new StringBuilder();
        sb.AppendLine(BoxTop());
        sb.AppendLine(BoxLine($"{title} [{queryId:D4}] {perfIcon} {ms:F1}ms - {kind}"));
        sb.AppendLine(BoxSep());
        sb.AppendLine(BoxLine($"📊 Database: {db}  |  🔗 Connection: {conn}  |  {tx}  |  ⏱️ Timeout: {cmd.CommandTimeout}s"));

        if (!string.IsNullOrWhiteSpace(resultHint))
            sb.AppendLine(BoxLine($"📋 {resultHint}"));

        if (_opt.IncludeParameters && cmd.Parameters.Count > 0)
        {
            sb.AppendLine(BoxLine($"🔧 Parameters: {Params(cmd)}"));
        }

        sb.AppendLine(BoxSep());
        sb.AppendLine(BoxLine("📝 SQL:"));

        // SQL satırlarını box içine yazma
        var sqlLines = sql.Split('\n');
        foreach (var line in sqlLines)
        {
            var trimmedLine = line.Trim();
            if (!string.IsNullOrEmpty(trimmedLine))
            {
                sb.AppendLine(BoxLine($"   {trimmedLine}"));
            }
        }

        sb.Append(BoxBottom());

        var logMessage = sb.ToString();
        var colorizedMessage = _opt.EnableAnsiColor ?
            ColorizeSqlBox(logMessage, titleColor) : logMessage;

        // Structured logging
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["QueryId"] = queryId,
            ["ExecutionTimeMs"] = ms,
            ["CommandType"] = kind,
            ["DatabaseName"] = db,
            ["ConnectionId"] = conn,
            ["IsSlow"] = slow
        });

        _logger.Log(level, "SQL Query Executed\n{SqlBlock}", colorizedMessage);

        if (_opt.EchoToConsole)
        {
            Console.WriteLine(colorizedMessage);
            Console.WriteLine(); // Ekstra boş satır
        }
    }

    // -------------------- HELPER METHODS --------------------
    private static string Short(string? s) =>
        string.IsNullOrEmpty(s) ? "?" : (s.Length <= 8 ? s : s[^8..]);

    private static string GetShortSql(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql)) return "Empty";

        sql = _ws.Replace(sql.Trim(), " ");
        return sql.Length > 60 ? sql[..57] + "..." : sql;
    }

    private static string Params(DbCommand cmd)
    {
        if (cmd.Parameters.Count == 0) return "None";

        var parts = new List<string>();
        foreach (DbParameter p in cmd.Parameters)
        {
            var value = p.Value switch
            {
                null or DBNull => "NULL",
                string s when s.Length > 100 => $"\"{s[..97]}...\"",
                string s => $"\"{s}\"",
                DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
                _ => p.Value.ToString()
            };

            parts.Add($"{p.ParameterName}={value}");
        }
        return string.Join(", ", parts);
    }

    private static string Pretty(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql)) return sql;

        sql = _ws.Replace(sql, " ").Trim();

        string[] keywords = {
            " SELECT ", " FROM ", " WHERE ", " GROUP BY ", " HAVING ",
            " ORDER BY ", " INNER JOIN ", " LEFT JOIN ", " RIGHT JOIN ",
            " FULL OUTER JOIN ", " CROSS JOIN ", " JOIN ", " UNION ",
            " UNION ALL ", " VALUES ", " ON ", " AND ", " OR ",
            " INSERT INTO ", " UPDATE ", " DELETE FROM ", " SET "
        };

        foreach (var keyword in keywords)
        {
            sql = Regex.Replace(sql, Regex.Escape(keyword),
                "\n" + keyword.Trim() + " ", RegexOptions.IgnoreCase);
        }

        return sql.Trim();
    }

    private static string Colorize(string text, ConsoleColor color)
    {
        var colorCode = color switch
        {
            ConsoleColor.Red => "\x1b[31m",
            ConsoleColor.Green => "\x1b[32m",
            ConsoleColor.Yellow => "\x1b[33m",
            ConsoleColor.Blue => "\x1b[34m",
            ConsoleColor.Magenta => "\x1b[35m",
            ConsoleColor.Cyan => "\x1b[36m",
            ConsoleColor.White => "\x1b[37m",
            ConsoleColor.Gray => "\x1b[90m",
            _ => "\x1b[37m"
        };

        return $"{colorCode}{text}\x1b[0m";
    }

    private string ColorizeSqlBox(string text, ConsoleColor titleColor)
    {
        if (!_opt.EnableAnsiColor) return text;

        var lines = text.Split('\n');
        var result = new StringBuilder();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (i == 0 || i == lines.Length - 1) // Top/bottom border
            {
                result.AppendLine(Colorize(line, ConsoleColor.Gray));
            }
            else if (line.Contains("SQL QUERY") || line.Contains("SLOW QUERY"))
            {
                result.AppendLine(Colorize(line, titleColor));
            }
            else if (line.Contains("SELECT") || line.Contains("FROM") ||
                     line.Contains("WHERE") || line.Contains("INSERT") ||
                     line.Contains("UPDATE") || line.Contains("DELETE"))
            {
                result.AppendLine(Colorize(line, ConsoleColor.Blue));
            }
            else if (line.Contains("├") || line.Contains("│"))
            {
                result.AppendLine(Colorize(line, ConsoleColor.Gray));
            }
            else
            {
                result.AppendLine(line);
            }
        }

        return result.ToString().TrimEnd();
    }

    // Box drawing characters
    private static string BoxTop() => "┌" + new string('─', 85) + "┐";
    private static string BoxSep() => "├" + new string('─', 85) + "┤";
    private static string BoxBottom() => "└" + new string('─', 85) + "┘";

    private static string BoxLine(string content)
    {
        const int maxWidth = 83;
        if (content.Length > maxWidth)
            content = content[..maxWidth];

        return "│ " + content.PadRight(maxWidth) + " │";
    }
}