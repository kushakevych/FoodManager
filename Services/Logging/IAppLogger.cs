using System;

namespace FoodManager.Services.Logging
{
    /// <summary>
    /// Contract for application logging.
    /// </summary>
    public interface IAppLogger
    {
        void Log(string message);
        void LogError(string message, Exception? ex = null);
        void Clear();
        string LogFilePath { get; }
    }
}
