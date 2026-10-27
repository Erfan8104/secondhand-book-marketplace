using System.Globalization;
using BookMarketplace.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BookMarketplace.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var statusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                AuthenticationFailedException => StatusCodes.Status401Unauthorized,
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                ConflictException => StatusCodes.Status409Conflict,
                AccountLockedException => StatusCodes.Status429TooManyRequests,
                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                logger.LogError(
                    exception,
                    "An unhandled exception occurred. TraceId: {TraceId}",
                    context.TraceIdentifier);
            }
            else
            {
                // Expected client errors (wrong password, duplicate phone...)
                // do not need a stack trace in the logs.
                logger.LogWarning(
                    "Request failed with {StatusCode}: {Message} TraceId: {TraceId}",
                    statusCode,
                    exception.Message,
                    context.TraceIdentifier);
            }

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode switch
                {
                    400 => "Bad Request",
                    401 => "Unauthorized",
                    403 => "Forbidden",
                    404 => "Resource Not Found",
                    409 => "Conflict",
                    429 => "Too Many Requests",
                    _ => "Internal Server Error"
                },
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message,
                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] =
                context.TraceIdentifier;

            if (exception is AccountLockedException locked)
            {
                var retryAfterSeconds = Math.Max(
                    1,
                    (int)Math.Ceiling(
                        (locked.LockoutEndUtc - DateTime.UtcNow).TotalSeconds));

                context.Response.Headers.RetryAfter =
                    retryAfterSeconds.ToString(CultureInfo.InvariantCulture);
            }

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}