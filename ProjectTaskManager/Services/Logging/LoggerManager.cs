using NLog;

namespace ProjectTaskManager.Services.Logging
{
   
    public class LoggerManager : ILoggerManager
    {
       private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public void LogInfo(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void LogWarning(string message)
        {
            _logger.Warn(message);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void LogError(Exception exception, string message)
        {
            _logger.Error(exception, message);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }

        public void LogFatal(string message)
        {
            _logger.Fatal(message);
        }

        public void LogFatal(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }

        public void LogFatal(Exception exception, string message)
        {
            _logger.Fatal(exception, message);
        }

        public void LogFatal(Exception exception, string message, params object[] args)
        {
            _logger.Fatal(exception, message, args);
        }
    }
}