using System;
using System.IO;
using System.Text;
using System.Threading;

namespace FoodManager
{
    // Very small thread-safe logger for debugging the price update process.
    // Writes to "price_update_debug.log" in the application folder.
    public static class DebugLogger
    {
        private static readonly object _lock = new object();
        private static readonly string _logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "price_update_debug.log");

        public static void Clear()
        {
            try
            {
                lock (_lock)
                {
                    File.WriteAllText(_logPath, $"=== price update debug log started {DateTime.UtcNow:O} ==={Environment.NewLine}", Encoding.UTF8);
                }
            }
            catch { /* ignore */ }
        }

        public static void Log(string message)
        {
            try
            {
                var line = $"{DateTime.UtcNow:O} [T{Thread.CurrentThread.ManagedThreadId}] {message}";
                lock (_lock)
                {
                    File.AppendAllText(_logPath, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch { /* ignore logging errors */ }
        }

        public static void LogError(string message, Exception? ex = null)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append($"{DateTime.UtcNow:O} [T{Thread.CurrentThread.ManagedThreadId}] ERROR: {message}");
                if (ex != null)
                {
                    sb.AppendLine();
                    sb.Append(ex.ToString());
                }
                lock (_lock)
                {
                    File.AppendAllText(_logPath, sb.ToString() + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch { /* ignore */ }
        }

        // Expose the log file path under a non-conflicting name
        public static string LogFilePath => _logPath;
    }
}