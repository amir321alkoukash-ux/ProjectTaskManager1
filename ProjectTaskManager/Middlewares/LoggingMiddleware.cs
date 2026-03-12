using System.Diagnostics;

namespace ProjectTaskManager.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Starting request: {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                await _next(context);

                stopwatch.Stop();
                _logger.LogInformation("Completed request: {Method} {Path} - {StatusCode} in {ElapsedMs}ms",
                    context.Request.Method, context.Request.Path,
                    context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Request failed: {Method} {Path} - Error: {ErrorMessage}",
                    context.Request.Method, context.Request.Path, ex.Message);
                throw;
            }
        }
    }
}