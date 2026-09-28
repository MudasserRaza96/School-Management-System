using System.Net;
using System.Text.Json;
using SchoolApiService.Models;

namespace SchoolApiService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing request path {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ArgumentException or InvalidOperationException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            // Sanitize error message to avoid exposing raw connection strings or passwords
            var errorMessage = exception.Message;
            if (errorMessage.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("ConnectionString", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("Server=", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "A database error occurred. Please contact the administrator.";
            }

            var apiResponse = ApiResponse<object>.ErrorResponse(
                message: statusCode == (int)HttpStatusCode.InternalServerError ? "An unexpected server error occurred." : errorMessage,
                errors: new List<string> { errorMessage },
                statusCode: statusCode
            );

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(apiResponse, jsonOptions);
            return context.Response.WriteAsync(json);
        }
    }
}
