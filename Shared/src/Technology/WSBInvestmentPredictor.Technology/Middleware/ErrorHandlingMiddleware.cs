using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace WSBInvestmentPredictor.Technology.Middleware;

/// <summary>
/// Middleware for handling exceptions in the application pipeline.
/// Provides centralized error handling and consistent error response format.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to process the HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    /// <param name="exception">The exception that was thrown.</param>
    /// <returns>A task that represents the asynchronous operation of writing the error response.</returns>
    /// <remarks>
    /// Maps different types of exceptions to appropriate HTTP status codes:
    /// - ArgumentException and InvalidOperationException -> 400 Bad Request
    /// - Other exceptions -> 500 Internal Server Error
    /// </remarks>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ArgumentException argEx => (StatusCodes.Status400BadRequest, argEx.Message),
            InvalidOperationException invOpEx => (StatusCodes.Status400BadRequest, invOpEx.Message),
            _ => (StatusCodes.Status500InternalServerError, "Wystąpił nieoczekiwany błąd serwera.")
        };

        context.Response.StatusCode = statusCode;

        var errorPayload = JsonSerializer.Serialize(new
        {
            error = message,
            status = statusCode
        });

        return context.Response.WriteAsync(errorPayload);
    }
}