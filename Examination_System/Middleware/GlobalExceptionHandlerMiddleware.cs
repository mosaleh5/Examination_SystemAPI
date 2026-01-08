using Examination_System.Models.Enums;
using Examination_System.ViewModels;
using System.Text.Json;
using System.Diagnostics;

namespace Examination_System.Middleware
{
    public class GlobalExceptionHandlerMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next, 
            ILogger<GlobalExceptionHandlerMiddleware> logger, 
            IWebHostEnvironment env)
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
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            // Map exception to status code AND error code
            (int statusCode, Models.Enums.ErrorCode errorCode) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, Models.Enums.ErrorCode.NotFound),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, Models.Enums.ErrorCode.Unauthorized),
                InvalidOperationException => (StatusCodes.Status400BadRequest, Models.Enums.ErrorCode.BadRequest),
                ArgumentNullException => (StatusCodes.Status400BadRequest, Models.Enums.ErrorCode.BadRequest),
                ArgumentException => (StatusCodes.Status400BadRequest, Models.Enums.ErrorCode.BadRequest),
                _ => (StatusCodes.Status500InternalServerError, Models.Enums.ErrorCode.InternalServerError)
            };
            
            context.Response.StatusCode = statusCode;

            // Create response using the errorCode from switch
            var response = new ResponseViewModel<object>
            {
                Data = null,
                IsSuccess = false,
                Message = _env.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred. Please try again later.",
                ErrorCode = errorCode, // ✅ Use errorCode from switch
               // Timestamp = DateTime.UtcNow,
               // TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var jsonResponse = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}