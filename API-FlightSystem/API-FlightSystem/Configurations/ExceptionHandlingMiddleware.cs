using Application.Common;
using Application.Exceptions;
using Serilog.Context;
using Shared.Logger;
using System.Text.Json;

namespace API_FlightBooking.Configurations
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var correlationId = httpContext.TraceIdentifier;
            httpContext.Response.Headers["X-Correlation-ID"] = correlationId;
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _next(httpContext);
                }
                catch (Exception ex)
                {
                    await HandleException(httpContext, ex);
                }
            }
        }

        private Task HandleException(HttpContext httpContext, Exception ex)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            var errors = ex.Message.Split("|\n\b|").ToList();
            statusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                ResourceNotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                ForgeException => StatusCodes.Status400BadRequest,
                UnprocessableRequestException => StatusCodes.Status422UnprocessableEntity,
                _ => statusCode
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                Logging.Error(ex, "Unhandled exception at {Path}", httpContext.Request.Path);
                errors = new List<string> { "Server Error" };
            }
            else
            {
                Logging.Warning("Handled exception at {Path} StatusCode {StatusCode} Message {Message}",
                    httpContext.Request.Path, statusCode, ex.Message);
            }

            var result = JsonSerializer.Serialize(ApiResult<string>.Failure(errors), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            return httpContext.Response.WriteAsync(result);
        }
    }
}
