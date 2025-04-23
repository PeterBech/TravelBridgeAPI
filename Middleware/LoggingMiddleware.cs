namespace TravelBridgeAPI.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var endpointName = endpoint?.DisplayName ?? "Unknown endpoint";
            // Log før request behandles
            Console.WriteLine($"[LOG] Request received: {DateTime.Now} - {context.Request.Method} {context.Request.Path} - Endpoint: {endpointName} ");

            // Kald næste middleware i pipelinen
            await _next(context);

            // Log efter request er behandlet
            Console.WriteLine($"[LOG] Response sent: {DateTime.Now} - Statuscode: {context.Response.StatusCode}");
        }
    }
}
