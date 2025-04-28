using System.Diagnostics;

namespace TravelBridgeAPI.Middleware
{
    public class LoggingMiddleware
    {
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private int _requestCount = 0;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _requestCount++;
            if (_requestCount == 101 )
                _requestCount = 0; // Resets the counter after 100 requests


            var endpoint = context.GetEndpoint();
            var endpointName = endpoint?.DisplayName ?? "Unknown endpoint";
            var method = context.Request.Method;
            var path = context.Request.Path;

            // Log before the request is processed
            // (Structured logging has now been implemented as part of the Serilog integration)
            // @RequestInfo, @ResponseInfo, and @ErrorInfo are Serilog templates
            // This allows Serilog to interpret the object as a structured object
            // It also ensures that Serilog stores the fields separately in the database

            _logger.LogInformation("Request started {@RequestInfo}", new
            {
                LogNumber = _requestCount,
                Timestamp = DateTime.UtcNow,
                Method = method,
                Path = path,
                Endpoint = endpointName
            });

            var sw = Stopwatch.StartNew();
            try
            {
                await _next(context);
                sw.Stop();

                var statuscode = context.Response.StatusCode;

                _logger.LogInformation("Response sent {@ResponseInfo}", new
                {
                    LogNumber = _requestCount,
                    Timestamp = DateTime.UtcNow,
                    StatusCode = statuscode,
                    ProcessingTimeMs = sw.ElapsedMilliseconds,
                    Method = method,
                    Path = path,
                    Endpoint = endpointName
                });

            }
            catch ( Exception ex)
            {
                sw.Stop();

                var statuscode = context.Response.StatusCode;

                _logger.LogError(ex, "Error occurred {@ErrorInfo}", new
                {
                    LogNumber = _requestCount,
                    Timestamp = DateTime.UtcNow,
                    StatusCode = statuscode,
                    ProcessingTimeMs = sw.ElapsedMilliseconds,
                    Method = method,
                    Path = path,
                    Endpoint = endpointName
                });
                throw;
            }
            
        }
    }
}
