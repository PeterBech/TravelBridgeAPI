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
            
            // Log før request behandles
            _logger.LogInformation($"[LOG] Log num: {_requestCount} Request started: {DateTime.Now} - {context.Request.Method} {context.Request.Path} - Endpoint: {endpointName} ");

            var sw = Stopwatch.StartNew();
            try
            {
                await _next(context);
                sw.Stop();
                _logger.LogInformation($"[LOG] Log num: {_requestCount} Response sent: {DateTime.Now} - Statuscode: {context.Response.StatusCode} - Processing time: {sw.ElapsedMilliseconds} ms");

            }
            catch ( Exception ex ) 
            {
                sw.Stop();
                _logger.LogError(ex, $"[LOG] Log num: {_requestCount} Error occurred: {DateTime.Now} - Statuscode: {context.Response.StatusCode} - Processing time: {sw.ElapsedMilliseconds} ms");
                throw;
            }
            
        }
    }
}
