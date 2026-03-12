namespace ProjectTaskManager.Services.Logging
{
    public interface ILoggerManager
    {
        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarn(string message);
        void LogError(string message);
        void LogError(Exception ex, string message);
    }
}