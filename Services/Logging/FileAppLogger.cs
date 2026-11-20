using System;
using System.IO;
using System.Text;
using System.Threading;

namespace FoodManager.Services.Logging
{
    /// <summary>
    /// Thread-safe file-based application logger.
    /// </summary>
    public class FileAppLogger : IAppLogger
    {
        private static readonly Lazy<FileAppLogger> _defaultInstance = new(() => new FileAppLogger());
        public static FileAppLogger Default => _defaultInstance.Value;

        private readonly object _lock = new();
        private readonly string _logPath;

        public FileAppLogger(string? customPath = null)
        {
            _logPath = customPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "price_update_debug.log");
        }

        public string LogFilePath => _logPath;

        public void Clear()
        {
            try
            {
                lock (_lock)
                {
                    File.WriteAllText(_logPath, $"=== price update debug log started {DateTime.UtcNow:O} ==={Environment.NewLine}", Encoding.UTF8);
                }
            }
            catch
            {
                // Ignore logging setup errors
            }
        }

        public void Log(string message)
        {
            try
            {
                var line = $"{DateTime.UtcNow:O} [T{Thread.CurrentThread.ManagedThreadId}] {message}";
                lock (_lock)
                {
                    File.AppendAllText(_logPath, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // Best-effort logging
            }
        }

        public void LogError(string message, Exception? ex = null)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append($"{DateTime.UtcNow:O} [T{Thread.CurrentThread.ManagedThreadId}] ERROR: {message}");
                if (ex != null)
                {
                    sb.AppendLine();
                    sb.Append(ex);
                }
                lock (_lock)
                {
                    File.AppendAllText(_logPath, sb.ToString() + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // Best-effort logging
            }
        }
    }
}
