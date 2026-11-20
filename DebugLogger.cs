using System;
using FoodManager.Services.Logging;

namespace FoodManager
{
    /// <summary>
    /// Thread-safe logger facade for debugging price updating and operations.
    /// Delegates to <see cref="FileAppLogger.Default"/>.
    /// </summary>
    public static class DebugLogger
    {
        public static void Clear() => FileAppLogger.Default.Clear();

        public static void Log(string message) => FileAppLogger.Default.Log(message);

        public static void LogError(string message, Exception? ex = null) => FileAppLogger.Default.LogError(message, ex);

        public static string LogFilePath => FileAppLogger.Default.LogFilePath;
    }
}
