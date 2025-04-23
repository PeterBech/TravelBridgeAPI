namespace TravelBridgeAPI.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
