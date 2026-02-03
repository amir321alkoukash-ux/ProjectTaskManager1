using System;

namespace ProjectTaskManager.Services.Logging
{
    /// <summary>
    /// Interface for logger manager service
    /// </summary>
    public interface ILoggerManager
    {
        // Debug level logging
        void LogDebug(string message);
        void LogDebug(string message, params object[] args);

        // Information level logging
        void LogInfo(string message);
        void LogInfo(string message, params object[] args);

        // Warning level logging
        void LogWarning(string message);
        void LogWarning(string message, params object[] args);

        // Error level logging
        void LogError(string message);
        void LogError(string message, params object[] args);
        void LogError(Exception exception, string message);
        void LogError(Exception exception, string message, params object[] args);

        // Fatal/Critical level logging
        void LogFatal(string message);
        void LogFatal(string message, params object[] args);
        void LogFatal(Exception exception, string message);
        void LogFatal(Exception exception, string message, params object[] args);
    }
}