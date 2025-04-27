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
                _requestCount = 0; // Resetter tælleren efter 100 requests


            var endpoint = context.GetEndpoint();
            var endpointName = endpoint?.DisplayName ?? "Unknown endpoint";
            var method = context.Request.Method;
            var path = context.Request.Path;

            // Log før request behandles 
            // (Her er nu implementeret structured logging, som led i implementeringen af serilog)
            // @RequestInfo, @ResponseInfo og @ErrorInfo er serilog templates
            // Dermed kan serilog læse objektet som et structured object
            // Dette gør også at Serilog gemmer felterne separat til DB
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
