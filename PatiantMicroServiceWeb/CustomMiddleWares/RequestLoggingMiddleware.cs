namespace PatiantMicroService.Web.CustomMiddleWares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var start = DateTime.UtcNow;

            _logger.LogInformation("Incoming Request: {method} {path}",
                context.Request.Method,
                context.Request.Path);

            await _next(context);

            var duration = DateTime.UtcNow - start;

            _logger.LogInformation("Response: {statusCode} in {duration} ms",
                context.Response.StatusCode,
                duration.TotalMilliseconds);
        }
    }
}
