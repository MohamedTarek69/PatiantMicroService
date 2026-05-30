using Microsoft.AspNetCore.Mvc;
using PatiantMicroService.Shared.CommonResult;
using System.Net;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PatiantMicroService.Web.CustomMiddleWares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var error = MapExceptionToError(ex);

            context.Response.StatusCode = error.ErrorType switch
            {
                ErrorType.NotFound => (int)HttpStatusCode.NotFound,
                ErrorType.Unauthorized => (int)HttpStatusCode.Unauthorized,
                ErrorType.Forbidden => (int)HttpStatusCode.Forbidden,
                ErrorType.Conflict => (int)HttpStatusCode.Conflict,
                ErrorType.Timeout => (int)HttpStatusCode.RequestTimeout,
                ErrorType.ServiceUnavailable => (int)HttpStatusCode.ServiceUnavailable,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                type = error.ErrorType.ToString(),
                title = error.Code,
                status = context.Response.StatusCode,
                detail = _env.IsDevelopment() ? ex.Message : error.Description,
                traceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }

        private static Shared.CommonResult.Error MapExceptionToError(Exception ex)
        {
            // تقدر تزود حالات خاصة هنا
            return ex switch
            {
                UnauthorizedAccessException =>
                    Shared.CommonResult.Error.Unauthorized(),

                KeyNotFoundException =>
                    Shared.CommonResult.Error.NotFound(),

                TimeoutException =>
                    Shared.CommonResult.Error.Timeout(),

                _ =>
                   Shared.CommonResult.Error.InternalServerError()
            };
        }
    }
}
